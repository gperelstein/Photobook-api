using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NJsonSchema.Annotations;
using Photobook.Data;
using Photobook.Common.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;
using Photobook.Common.HandlersResponses;
using System.Net;

namespace Photobook.Logic.Features.Users
{
    public class RegisterUser
    {
        [JsonSchema("RegisterUserCommand")]
        public class Command : IRequest<Response<Unit>>
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string Token { get; set; }
        }

        public class Handler : IRequestHandler<Command, Response<Unit>>
        {
            private readonly UserManager<PhotobookUser> _userManager;
            private readonly PhotobookDbContext _context;

            public Handler(UserManager<PhotobookUser> userManager, PhotobookDbContext context)
            {
                _userManager = userManager;
                _context = context;
            }
            public async Task<Response<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email);

                    if (user == null)
                    {
                        return new Response<Unit>(
                            new Error(ErrorCodes.InvalidUser,
                            ErrorMessages.InvalidUser,
                            HttpStatusCode.BadRequest));
                    }

                    var result = await _userManager.ConfirmEmailAsync(user, request.Token);

                    if (!result.Succeeded)
                    {
                        return new Response<Unit>(
                            new Error(ErrorCodes.InvalidToken,
                            ErrorMessages.InvalidToken,
                            HttpStatusCode.BadRequest));
                    }

                    await _userManager.AddPasswordAsync(user, request.Password);

                    return new Response<Unit>(Unit.Value);
                }
                catch(Exception ex)
                {
                    return new Response<Unit>(
                            new Error(ErrorCodes.UnexpectedError,
                            ex.Message,
                            HttpStatusCode.BadRequest));
                }
            }
        }
    }
}
