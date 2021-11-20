using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Photobook.Common.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Photobook.Logic.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IUserClaimsPrincipalFactory<PhotobookUser> _claimsFactory;
        private readonly UserManager<PhotobookUser> _userManager;
        private readonly SignInManager<PhotobookUser> _signInManager;
        public ProfileService(UserManager<PhotobookUser> userManager,
            IUserClaimsPrincipalFactory<PhotobookUser> claimsFactory,
            SignInManager<PhotobookUser> signInManager)
        {
            _userManager = userManager;
            _claimsFactory = claimsFactory;
            _signInManager = signInManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);

            if (user == null)
            {
                await _signInManager.SignOutAsync();
                return;
            }
            var principal = await _claimsFactory.CreateAsync(user);

            var issuedClaims = new List<Claim>
            {
                new Claim("userId", user.Id.ToString()),
                GetRolesClaim(context, principal),
            };
            issuedClaims.AddRange(GetProfileClaims(context, principal, user));

            if (issuedClaims.Any())
            {
                context.IssuedClaims.AddRange(issuedClaims);
            }
        }

        private List<Claim> GetProfileClaims(ProfileDataRequestContext context, ClaimsPrincipal principal, PhotobookUser user)
        {
            var claimTypes = new string[]
            {
                JwtClaimTypes.Name,
                JwtClaimTypes.Email,
                JwtClaimTypes.Id
            };
            var claims = principal.Claims.Where(c => claimTypes.Any(x => x == c.Type)).ToList();
            claims.Add(new Claim(JwtClaimTypes.PreferredUserName, user.UserName));
            return claims;
        }

        private Claim GetRolesClaim(ProfileDataRequestContext context, ClaimsPrincipal principal)
        {
            var roleClaims = string.Join(",", principal.Claims.Where(c => c.Type == "role").Select(c => c.Value).ToList());
            return new Claim("roles", roleClaims);
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var userId = context.Subject.GetSubjectId();
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == new Guid(userId));

            if (user == null)
            {
                context.IsActive = false;
                return;
            }

            if (!user.IsActive)
                throw new Exception("This account is currently deactivated.");

            context.IsActive = true;
        }
    }
}
