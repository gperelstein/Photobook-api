using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NJsonSchema.Annotations;
using Photobook.Data;
using Photobook.Models.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Photobook.Logic.Features.Users
{
    public class RegisterUser
    {
        [JsonSchema("Command")]
        public class Command : IRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string Token { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly UserManager<PhotobookUser> _userManager;
            private readonly PhotobookDbContext _context;

            public Handler(UserManager<PhotobookUser> userManager, PhotobookDbContext context)
            {
                _userManager = userManager;
                _context = context;
            }
            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email);

                if(user == null)
                {
                    return Unit.Value;
                }

                var result = await _userManager.ConfirmEmailAsync(user, request.Token);

                if(!result.Succeeded)
                {
                    return Unit.Value;
                }

                await _userManager.AddPasswordAsync(user, request.Password);

                return Unit.Value;
            }
        }
    }
}
