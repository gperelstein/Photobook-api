using Microsoft.AspNetCore.Identity;
using Moq;
using Photobook.Common.HandlersResponses;
using Photobook.Common.Identity;
using Photobook.Data;
using Photobook.Logic.Features.Users;
using Photobook.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Photobook.Tests.Logic.Features.Users
{
    public class ResetPasswordHandlerTests
    {
        private readonly Mock<UserManager<PhotobookUser>> _userManager;
        private readonly PhotobookDbContext _context;
        private readonly ResetPassword.Handler _classUnderTest;

        public ResetPasswordHandlerTests()
        {
            _userManager = UserManagerHelper.MockUserManager<PhotobookUser>(new List<PhotobookUser>());
            _context = ContextHelper.CreateContext();
            _classUnderTest = new ResetPassword.Handler(_userManager.Object, _context);
        }

        [Fact]
        public async Task Handle_ShouldReturnsSuccessResponse_WhenSucceed()
        {
            var email = "Email";
            _userManager.Setup(x => x.ResetPasswordAsync(It.IsAny<PhotobookUser>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            var user = new PhotobookUser
            {
                Email = email,
                IsActive = true
            };
            await _context.AddAsync(user);
            await _context.SaveChangesAsync();
            var command = new ResetPassword.Command
            {
                Email = email,
                Password = "Password",
                Token = "Token"
            };

            var result = await _classUnderTest.Handle(command, default);

            Assert.True(result.Success);
        }

        [Fact]
        public async Task Handle_ShouldReturnsInvalidUserError_WhenUserNotExists()
        {
            var email = "Email";
            var command = new ResetPassword.Command
            {
                Email = email,
                Password = "Password",
                Token = "Token"
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
            var email = "Email";
            _userManager.Setup(x => x.ResetPasswordAsync(It.IsAny<PhotobookUser>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            var user = new PhotobookUser
            {
                Email = email,
                IsActive = false
            };
            await _context.AddAsync(user);
            await _context.SaveChangesAsync();
            var command = new ResetPassword.Command
            {
                Email = email,
                Password = "Password",
                Token = "Token"
            };

            var result = await _classUnderTest.Handle(command, default);

            Assert.False(result.Success);
            Assert.Equal(ErrorCodes.UserNotActive, result.Error.Code);
            Assert.Equal(ErrorMessages.UserNotActive, result.Error.Message);
            Assert.Equal(HttpStatusCode.BadRequest, result.Error.StatusCode);
        }

        [Fact]
        public async Task Handle_ShouldReturnsUnexpectedErrorError_WhenResetPasswordFails()
        {
            var email = "Email";
            _userManager.Setup(x => x.ResetPasswordAsync(It.IsAny<PhotobookUser>(),
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
            var user = new PhotobookUser
            {
                Email = email,
                IsActive = true
            };
            await _context.AddAsync(user);
            await _context.SaveChangesAsync();
            var command = new ResetPassword.Command
            {
                Email = email,
                Password = "Password",
                Token = "Token"
            };

            var result = await _classUnderTest.Handle(command, default);

            Assert.False(result.Success);
            Assert.Equal(ErrorCodes.UnexpectedError, result.Error.Code);
            Assert.Equal(ErrorMessages.UnexpectedError, result.Error.Message);
            Assert.Equal(HttpStatusCode.InternalServerError, result.Error.StatusCode);
        }

        [Fact]
        public async Task Handle_ShouldReturnsUnexpectedErrorError_WhenAnExceptionIsThrown()
        {
            var email = "Email";
            var exception = new Exception("Message");
            _userManager.Setup(x => x.ResetPasswordAsync(It.IsAny<PhotobookUser>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
                .Throws(exception);
            var user = new PhotobookUser
            {
                Email = email,
                IsActive = true
            };
            await _context.AddAsync(user);
            await _context.SaveChangesAsync();
            var command = new ResetPassword.Command
            {
                Email = email,
                Password = "Password",
                Token = "Token"
            };

            var result = await _classUnderTest.Handle(command, default);

            Assert.False(result.Success);
            Assert.Equal(ErrorCodes.UnexpectedError, result.Error.Code);
            Assert.Equal(exception.Message, result.Error.Message);
            Assert.Equal(HttpStatusCode.InternalServerError, result.Error.StatusCode);
        }
    }
}
