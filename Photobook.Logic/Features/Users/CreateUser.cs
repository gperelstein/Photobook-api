using MediatR;
using Microsoft.AspNetCore.Identity;
using Photobook.Logic.Features.Users.Responses;
using Photobook.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Photobook.Logic.Features.Users
{
    public class CreateUser
    {
        public class Command : IRequest<UserResponse>
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
        }

        public class Handler : IRequestHandler<Command, UserResponse>
        {
            private readonly UserManager<PhotobookUser> _userManager;

            public Handler(UserManager<PhotobookUser> userManager)
            {
                _userManager = userManager;
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

                if(!result.Succeeded)
                {
                    return new UserResponse();
                }

                return new UserResponse
                {
                    Email = newUser.Email
                };
            }
        }
    }
}
