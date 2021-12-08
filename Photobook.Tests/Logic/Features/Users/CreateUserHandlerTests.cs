using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using Photobook.Common.Configuration;
using Photobook.Common.HandlersResponses;
using Photobook.Common.Identity;
using Photobook.Data;
using Photobook.Logic.Features.Users;
using Photobook.Notifications;
using Photobook.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Photobook.Tests.Logic.Features.Users
{
    public class CreateUserHandlerTests
    {
        private readonly Mock<UserManager<PhotobookUser>> _userManager;
        private readonly Mock<INotificationController> _notificationController;
        private readonly Mock<IOptions<AppOptions>> _options;
        private readonly PhotobookDbContext _context;
        private readonly CreateUser.Handler _classUnderTest;

        public CreateUserHandlerTests()
        {
            _userManager = UserManagerHelper.MockUserManager<PhotobookUser>(new List<PhotobookUser>());
            _notificationController = new Mock<INotificationController>();
            _options = new Mock<IOptions<AppOptions>>();
            _options.Setup(x => x.Value)
                .Returns(new AppOptions
                {
                    Urls = new UrlsOptions
                    {
                        WebUrl = "http://localhost"
                    }
                });
            _context = ContextHelper.CreateContext();
            _classUnderTest = new CreateUser.Handler(_userManager.Object,
                _notificationController.Object,
                _options.Object,
                _context);
        }

        [Fact]
        public async Task Handle_ShouldCreateUser_WhenSucceed()
        {
            var token = "token";
            _userManager.Setup(x => x.CreateAsync(It.IsAny<PhotobookUser>()))
                .ReturnsAsync(IdentityResult.Success);
            _userManager.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<PhotobookUser>()))
                .ReturnsAsync(token);
            var command = new CreateUser.Command
            {
                FirstName = "FirstName",
                LastName = "LastName",
                Email = "Email"
            };

            var result = await _classUnderTest.Handle(command, default);

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == command.Email);
            var profile = await _context.Profiles.FirstOrDefaultAsync(x => x.FirstName == command.FirstName);
            Assert.True(result.Success);
            Assert.Equal(command.Email, result.Value.Email);
            Assert.False(user.IsActive);
            Assert.Equal(command.Email, user.UserName);
            Assert.Equal(command.LastName, profile.LastName);
        }

        [Fact]
        public async Task Handle_ShouldReturnsEmailAlreadyExistsError_WhenCreateUserAlreadyExists()
        {
            
            _userManager.Setup(x => x.CreateAsync(It.IsAny<PhotobookUser>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError[]
                {
                    new IdentityError
                    {
                        Code = "IdentityErrorCode",
                        Description = "IdentityErrorDescription"
                    }
                }));
            var command = new CreateUser.Command
            {
                FirstName = "FirstName",
                LastName = "LastName",
                Email = "Email"
            };

            var result = await _classUnderTest.Handle(command, default);

            Assert.False(result.Success);
            Assert.Equal(ErrorCodes.EmailAlreadyExists, result.Error.Code);
            Assert.Equal(ErrorMessages.EmailAlreadyExists, result.Error.Message);
            Assert.Equal(HttpStatusCode.BadRequest, result.Error.StatusCode);
        }

        [Fact]
        public async Task Handle_ShouldReturnsUnexpectedError_WhenAnExceptionIsThrown()
        {
            var exception = new Exception("Exception");
            _userManager.Setup(x => x.CreateAsync(It.IsAny<PhotobookUser>()))
                .Throws(exception);
            var command = new CreateUser.Command
            {
                FirstName = "FirstName",
                LastName = "LastName",
                Email = "Email"
            };

            var result = await _classUnderTest.Handle(command, default);

            Assert.False(result.Success);
            Assert.Equal(ErrorCodes.UnexpectedError, result.Error.Code);
            Assert.Equal(exception.Message, result.Error.Message);
            Assert.Equal(HttpStatusCode.InternalServerError, result.Error.StatusCode);
        }
    }
}
