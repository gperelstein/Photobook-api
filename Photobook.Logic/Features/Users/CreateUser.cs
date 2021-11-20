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

namespace Photobook.Logic.Features.Users
{
    public class CreateUser
    {
        [JsonSchema("CreateUserCommand")]
        public class Command : IRequest<Response<UserResponse>>
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
        }

        public class Handler : IRequestHandler<Command, Response<UserResponse>>
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

            public async Task<Response<UserResponse>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var newUser = new PhotobookUser
                    {
                        Email = request.Email,
                        UserName = request.Email,
                        IsActive = false
                    };

                    var result = await _userManager.CreateAsync(newUser);

                    if (!result.Succeeded)
                    {
                        return new Response<UserResponse>(
                            new Error(ErrorCodes.EmailAlreadyExists,
                            ErrorMessages.EmailAlreadyExists,
                            HttpStatusCode.BadRequest));
                    }

                    var profile = new Profile
                    {
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        User = newUser
                    };

                    await _context.Profiles.AddAsync(profile);
                    await _context.SaveChangesAsync();

                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);

                    var notification = CreateNotification(token, request.Email, request.FirstName);

                    await _notificationController.PushAsync(notification);

                    var user = new UserResponse { Email = newUser.Email };

                    return new Response<UserResponse>(user);
                }
                catch(Exception ex)
                {
                    return new Response<UserResponse>(
                            new Error(ErrorCodes.UnexpectedError,
                            ex.Message,
                            HttpStatusCode.InternalServerError));
                }
            }

            private EmailNotification CreateNotification(string token, string email, string firstName) => new UserRegistration
            {
                RegistrationLink = _urlsConfig.GenerateLink($"registerCompletion?token={token}&email={email}"),
                To = email,
                FirstName = firstName,
                Subject = "Bienvenido a Photobook!!"
            };
        }
    }
}
