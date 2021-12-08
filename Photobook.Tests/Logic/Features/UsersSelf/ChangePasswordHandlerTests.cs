using Microsoft.AspNetCore.Identity;
using Moq;
using Photobook.Common.HandlersResponses;
using Photobook.Common.Identity;
using Photobook.Common.Services.CurrentUser;
using Photobook.Data;
using Photobook.Logic.Features.UsersSelf;
using Photobook.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Photobook.Tests.Logic.Features.UsersSelf
{
    public class ChangePasswordHandlerTests
    {
        private readonly Mock<UserManager<PhotobookUser>> _userManager;
        private readonly PhotobookDbContext _context;
        private readonly Mock<ICurrentUserService> _currentUserService;
        private readonly ChangePassword.Handler _classUnderTest;

        public ChangePasswordHandlerTests()
        {
            _userManager = UserManagerHelper.MockUserManager<PhotobookUser>(new List<PhotobookUser>());
            _context = ContextHelper.CreateContext();
            _currentUserService = new Mock<ICurrentUserService>();
            _classUnderTest = new ChangePassword.Handler(_currentUserService.Object,
                _context,
                _userManager.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnsSuccessResponse_WhenSucceed()
        {
            var user = new PhotobookUser
            {
                Email = "Email",
                IsActive = true
            };
            await _context.AddAsync(user);
            await _context.SaveChangesAsync();
            _userManager.Setup(x => x.ChangePasswordAsync(It.IsAny<PhotobookUser>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            _currentUserService.Setup(x => x.GetUserId())
                .Returns(user.Id);
            var command = new ChangePassword.Command
            {
                CurrentPassword = "CurrentPassword",
                NewPassword = "NewPassword"
            };

            var result = await _classUnderTest.Handle(command, default);

            Assert.True(result.Success);
        }

        [Fact]
        public async Task Handle_ShouldReturnsInvalidUserError_WhenUserNotExists()
        {
            _currentUserService.Setup(x => x.GetUserId())
                .Returns(Guid.NewGuid());
            var command = new ChangePassword.Command
            {
                CurrentPassword = "CurrentPassword",
                NewPassword = "NewPassword"
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
            var user = new PhotobookUser
            {
                Email = "Email",
                IsActive = false
            };
            await _context.AddAsync(user);
            await _context.SaveChangesAsync();
            _currentUserService.Setup(x => x.GetUserId())
                .Returns(user.Id);
            var command = new ChangePassword.Command
            {
                CurrentPassword = "CurrentPassword",
                NewPassword = "NewPassword"
            };

            var result = await _classUnderTest.Handle(command, default);

            Assert.False(result.Success);
            Assert.Equal(ErrorCodes.UserNotActive, result.Error.Code);
            Assert.Equal(ErrorMessages.UserNotActive, result.Error.Message);
            Assert.Equal(HttpStatusCode.BadRequest, result.Error.StatusCode);
        }

        [Fact]
        public async Task Handle_ShouldReturnsUnexpectedError_WhenChangePasswordFails()
        {
            var user = new PhotobookUser
            {
                Email = "Email",
                IsActive = true
            };
            await _context.AddAsync(user);
            await _context.SaveChangesAsync();
            _userManager.Setup(x => x.ChangePasswordAsync(It.IsAny<PhotobookUser>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError[]
                {
                    new IdentityError
                    {
                        Code = "IdentityErrorCode",
                        Description = "IdentityErrorDescription"
                    }
                }));
            _currentUserService.Setup(x => x.GetUserId())
                .Returns(user.Id);
            var command = new ChangePassword.Command
            {
                CurrentPassword = "CurrentPassword",
                NewPassword = "NewPassword"
            };

            var result = await _classUnderTest.Handle(command, default);

            Assert.False(result.Success);
            Assert.Equal(ErrorCodes.UnexpectedError, result.Error.Code);
            Assert.Equal(ErrorMessages.UnexpectedError, result.Error.Message);
            Assert.Equal(HttpStatusCode.InternalServerError, result.Error.StatusCode);
        }

        [Fact]
        public async Task Handle_ShouldReturnsUnexpectedError_WhenAnExceptionIsThrown()
        {
            var exception = new Exception("Message");
            var user = new PhotobookUser
            {
                Email = "Email",
                IsActive = true
            };
            await _context.AddAsync(user);
            await _context.SaveChangesAsync();
            _userManager.Setup(x => x.ChangePasswordAsync(It.IsAny<PhotobookUser>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
                .Throws(exception);
            _currentUserService.Setup(x => x.GetUserId())
                .Returns(user.Id);
            var command = new ChangePassword.Command
            {
                CurrentPassword = "CurrentPassword",
                NewPassword = "NewPassword"
            };

            var result = await _classUnderTest.Handle(command, default);

            Assert.False(result.Success);
            Assert.Equal(ErrorCodes.UnexpectedError, result.Error.Code);
            Assert.Equal(exception.Message, result.Error.Message);
            Assert.Equal(HttpStatusCode.InternalServerError, result.Error.StatusCode);
        }
    }
}
