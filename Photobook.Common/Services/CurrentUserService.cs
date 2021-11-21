using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;

namespace Photobook.Common.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? GetUserId()
        {
            var userSub = _httpContextAccessor.HttpContext?.User?.FindFirstValue("sub");
            if (Guid.TryParse(userSub, out Guid userId))
            {
                return userId;
            }

            return null;
        }
    }
}
