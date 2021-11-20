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

namespace Photobook.Logic.Features.Users
{
    public class CreateUser
    {
        [JsonSchema("ListCompanyUsersCommand")]
        public class Command : IRequest<UserResponse>
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
        }

        public class Handler : IRequestHandler<Command, UserResponse>
        {
            private readonly UserManager<PhotobookUser> _userManager;
            private readonly INotificationController _notificationController;

            public Handler(UserManager<PhotobookUser> userManager, INotificationController notificationController)
            {
                _userManager = userManager;
                _notificationController = notificationController;
            }

            public async Task<UserResponse> Handle(Command request, CancellationToken cancellationToken)
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
                    return new UserResponse();
                }

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);

                var notification = CreateNotification(token, request.Email, request.FirstName);

                await _notificationController.PushAsync(notification);

                return new UserResponse
                {
                    Email = newUser.Email
                };
            }

            private EmailNotification CreateNotification(string token, string email, string firstName) => new UserRegistration
            {
                RegistrationLink = token,
                To = email,
                FirstName = firstName
            };
        }
    }
}
