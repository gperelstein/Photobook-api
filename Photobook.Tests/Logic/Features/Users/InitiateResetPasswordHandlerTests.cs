using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using Photobook.Common.Configuration;
using Photobook.Common.HandlersResponses;
using Photobook.Common.Identity;
using Photobook.Common.Models;
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
    public class InitiateResetPasswordHandlerTests
    {
        private readonly Mock<UserManager<PhotobookUser>> _userManager;
        private readonly Mock<INotificationController> _notificationController;
        private readonly Mock<IOptions<AppOptions>> _options;
        private readonly PhotobookDbContext _context;
        private readonly InitiatePasswordReset.Handler _classUnderTest;

        public InitiateResetPasswordHandlerTests()
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
            _classUnderTest = new InitiatePasswordReset.Handler(_userManager.Object,
                _notificationController.Object,
                _options.Object,
                _context);
        }

        [Fact]
        public async Task Handle_ShouldReturnsSuccessResponse_WhenSucceed()
        {
            var token = "token";
            var email = "userEmail";
            _userManager.Setup(x => x.GeneratePasswordResetTokenAsync(It.IsAny<PhotobookUser>()))
                .ReturnsAsync(token);
            var profile = new Profile
            {
                FirstName = "FirstName",
                User = new PhotobookUser
                {
                    Email = email,
                    IsActive = true
                }
            };
            await _context.AddAsync(profile);
            await _context.SaveChangesAsync();
            var command = new InitiatePasswordReset.Command
            {
                Email = email
            };

            var result = await _classUnderTest.Handle(command, default);

            Assert.True(result.Success);
        }

        [Fact]
        public async Task Handle_ShouldReturnsInvalidUserError_WhenProfileNotExists()
        {
            var token = "token";
            var email = "userEmail";
            _userManager.Setup(x => x.GeneratePasswordResetTokenAsync(It.IsAny<PhotobookUser>()))
                .ReturnsAsync(token);
            var command = new InitiatePasswordReset.Command
            {
                Email = email
            };

            var result = await _classUnderTest.Handle(command, default);

            Assert.False(result.Success);
            Assert.Equal(ErrorCodes.InvalidUser, result.Error.Code);
            Assert.Equal(ErrorMessages.InvalidUser, result.Error.Message);
            Assert.Equal(HttpStatusCode.BadRequest, result.Error.StatusCode);
        }

        [Fact]
        public async Task Handle_ShouldReturnsUserNotActiveError_WhenUserIsNotActive()
        {
            var token = "token";
            var email = "userEmail";
            _userManager.Setup(x => x.GeneratePasswordResetTokenAsync(It.IsAny<PhotobookUser>()))
                .ReturnsAsync(token);
            var profile = new Profile
            {
                User = new PhotobookUser
                {
                    Email = email,
                    IsActive = false
                }
            };
            await _context.AddAsync(profile);
            await _context.SaveChangesAsync();
            var command = new InitiatePasswordReset.Command
            {
                Email = email
            };

            var result = await _classUnderTest.Handle(command, default);

            Assert.False(result.Success);
            Assert.Equal(ErrorCodes.UserNotActive, result.Error.Code);
            Assert.Equal(ErrorMessages.UserNotActive, result.Error.Message);
            Assert.Equal(HttpStatusCode.BadRequest, result.Error.StatusCode);
        }

        [Fact]
        public async Task Handle_ShouldReturnsUnexpectedError_WhenAnExceptionIsThrown()
        {
            var exception = new Exception("Message");
            var mockContext = ContextHelper.CreateMockContext();
            mockContext.Setup(x => x.Profiles)
                .Throws(exception);
            var classUnderTest = new InitiatePasswordReset.Handler(_userManager.Object,
                _notificationController.Object,
                _options.Object,
                mockContext.Object);
            var command = new InitiatePasswordReset.Command
            {
                Email = "Email"
            };

            var result = await classUnderTest.Handle(command, default);

            Assert.False(result.Success);
            Assert.Equal(ErrorCodes.UnexpectedError, result.Error.Code);
            Assert.Equal(exception.Message, result.Error.Message);
            Assert.Equal(HttpStatusCode.InternalServerError, result.Error.StatusCode);
        }
    }
}
