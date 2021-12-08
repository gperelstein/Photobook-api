using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NJsonSchema.Annotations;
using Photobook.Common.HandlersResponses;
using Photobook.Common.Identity;
using Photobook.Data;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Photobook.Logic.Features.Users
{
    public class ResetPassword
    {
        [JsonSchema("ResetPasswordCommand")]
        public class Command : IRequest<Response<Unit>>
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string Token { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Email)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty()
                    .EmailAddress();

                RuleFor(x => x.Password)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty()
                    .Matches(@"\d")
                    .Matches(@"[a-z]")
                    .Matches(@"[A-Z]")
                    .Matches(@"[\^$*.[\]{}(\)?""!@#%&/\\,><':;|_~`]");

                RuleFor(x => x.Token)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty();
            }
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
                    var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken);

                    if (user == null)
                    {
                        return new Response<Unit>(
                            new Error(ErrorCodes.InvalidUser,
                            ErrorMessages.InvalidUser,
                            HttpStatusCode.BadRequest));
                    }

                    if (!user.IsActive)
                    {
                        return new Response<Unit>(
                            new Error(ErrorCodes.UserNotActive,
                            ErrorMessages.UserNotActive,
                            HttpStatusCode.BadRequest));
                    }

                    var result = await _userManager.ResetPasswordAsync(user, request.Token, request.Password);

                    if (!result.Succeeded)
                    {
                        return new Response<Unit>(
                            new Error(ErrorCodes.UnexpectedError,
                            ErrorMessages.UnexpectedError,
                            HttpStatusCode.InternalServerError));
                    }

                    return new Response<Unit>(Unit.Value);
                }
                catch (Exception ex)
                {
                    return new Response<Unit>(
                            new Error(ErrorCodes.UnexpectedError,
                            ex.Message,
                            HttpStatusCode.InternalServerError));
                }
            }
        }
    }
}
