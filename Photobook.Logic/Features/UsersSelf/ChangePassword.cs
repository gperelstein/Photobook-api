using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NJsonSchema.Annotations;
using Photobook.Common.HandlersResponses;
using Photobook.Common.Identity;
using Photobook.Common.Services.CurrentUser;
using Photobook.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Photobook.Logic.Features.UsersSelf
{
    public class ChangePassword
    {
        [JsonSchema("ChangePasswordCommand")]
        public class Command : IRequest<Response<Unit>>
        {
            public string CurrentPassword { get; set; }
            public string NewPassword { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.CurrentPassword)
                        .Cascade(CascadeMode.Stop)
                        .NotEmpty();

                RuleFor(x => x.NewPassword)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty()
                    .Matches(@"\d")
                    .Matches(@"[a-z]")
                    .Matches(@"[A-Z]")
                    .Matches(@"[\^$*.[\]{}(\)?""!@#%&/\\,><':;|_~`]");
            }
        }

        public class Handler : IRequestHandler<Command, Response<Unit>>
        {
            private readonly ICurrentUserService _currentUserService;
            private readonly PhotobookDbContext _context;
            private readonly UserManager<PhotobookUser> _userManager;

            public Handler(ICurrentUserService currentUserService,
                PhotobookDbContext context,
                UserManager<PhotobookUser> userManager)
            {
                _currentUserService = currentUserService;
                _context = context;
                _userManager = userManager;
            }

            public async Task<Response<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var userId = _currentUserService.GetUserId();

                    var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

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

                    var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

                    if (!result.Succeeded)
                    {
                        return new Response<Unit>(
                            new Error(ErrorCodes.UnexpectedError,
                            ErrorMessages.UnexpectedError,
                            HttpStatusCode.InternalServerError));
                    }

                    return new Response<Unit>(Unit.Value);
                }
                catch(Exception ex)
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
