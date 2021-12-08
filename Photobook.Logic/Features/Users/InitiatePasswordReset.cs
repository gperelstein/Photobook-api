using MediatR;
using Microsoft.AspNetCore.Identity;
using NJsonSchema.Annotations;
using Photobook.Logic.Features.Users.Responses;
using Photobook.Common.Identity;
using Photobook.Notifications;
using Photobook.Notifications.Models;
using Photobook.Notifications.Templates;
using System.Threading;
using System.Threading.Tasks;
using Photobook.Common.HandlersResponses;
using System.Net;
using System;
using Photobook.Common.Configuration;
using Microsoft.Extensions.Options;
using Photobook.Data;
using Photobook.Common.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Photobook.Logic.Features.Users
{
    public class InitiatePasswordReset
    {
        [JsonSchema("InitiatePasswordResetCommand")]
        public class Command : IRequest<Response<Unit>>
        {
            public string Email { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Email)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty()
                    .EmailAddress();
            }
        }

        public class Handler : IRequestHandler<Command, Response<Unit>>
        {
            private readonly UserManager<PhotobookUser> _userManager;
            private readonly INotificationController _notificationController;
            private readonly UrlsOptions _urlsConfig;
            private readonly PhotobookDbContext _context;

            public Handler(UserManager<PhotobookUser> userManager,
                INotificationController notificationController,
                IOptions<AppOptions> options,
                PhotobookDbContext context)
            {
                _userManager = userManager;
                _notificationController = notificationController;
                _urlsConfig = options.Value.Urls;
                _context = context;
            }

            public async Task<Response<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var userProfile = await _context.Profiles.FirstOrDefaultAsync(x => x.User.Email == request.Email, cancellationToken);

                    if (userProfile == null)
                    {
                        return new Response<Unit>(
                            new Error(ErrorCodes.InvalidUser,
                            ErrorMessages.InvalidUser,
                            HttpStatusCode.BadRequest));
                    }
                    
                    if (!userProfile.User.IsActive)
                    {
                        return new Response<Unit>(
                            new Error(ErrorCodes.UserNotActive,
                            ErrorMessages.UserNotActive,
                            HttpStatusCode.BadRequest));
                    }

                    var token = await _userManager.GeneratePasswordResetTokenAsync(userProfile.User);

                    var notification = CreateNotification(token, userProfile.User.Email, userProfile.FirstName);

                    await _notificationController.PushAsync(notification);

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

            private EmailNotification CreateNotification(string token, string email, string firstName) => new InitiatePasswordResetNotification
            {
                ResetPasswordLink = _urlsConfig.GenerateLink($"resetPassword?token={token}&email={email}"),
                To = email,
                FirstName = firstName,
                Subject = "Resetea tu password"
            };
        }
    }
}
