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
    public class RegisterUserHandlerTests
    {
        private readonly Mock<UserManager<PhotobookUser>> _userManager;
        private readonly PhotobookDbContext _context;
        private readonly RegisterUser.Handler _classUnderTest;

        public RegisterUserHandlerTests()
        {
            _userManager = UserManagerHelper.MockUserManager<PhotobookUser>(new List<PhotobookUser>());
            _context = ContextHelper.CreateContext();
            _classUnderTest = new RegisterUser.Handler(_userManager.Object, _context);
        }

        [Fact]
        public async Task Handle_ShouldReturnsSuccessResponse_WhenSucceed()
        {
            var email = "Email";
            _userManager.Setup(x => x.ConfirmEmailAsync(It.IsAny<PhotobookUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            var user = new PhotobookUser
            {
                Email = email,
                IsActive = false
            };
            await _context.AddAsync(user);
            await _context.SaveChangesAsync();
            var command = new RegisterUser.Command
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
            var command = new RegisterUser.Command
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
        public async Task Handle_ShouldReturnsUserAlreadyRegisterError_WhenTheUserIsAlreadyRegister()
        {
            var email = "Email";
            var user = new PhotobookUser
            {
                Email = email,
                IsActive = true
            };
            await _context.AddAsync(user);
            await _context.SaveChangesAsync();
            var command = new RegisterUser.Command
            {
                Email = email,
                Password = "Password",
                Token = "Token"
            };

            var result = await _classUnderTest.Handle(command, default);

            Assert.False(result.Success);
            Assert.Equal(ErrorCodes.UserAlreadyRegister, result.Error.Code);
            Assert.Equal(ErrorMessages.UserAlreadyRegister, result.Error.Message);
            Assert.Equal(HttpStatusCode.BadRequest, result.Error.StatusCode);
        }

        [Fact]
        public async Task Handle_ShouldReturnsInvalidTokenError_WhenEmailConfirmationFails()
        {
            var email = "Email";
            _userManager.Setup(x => x.ConfirmEmailAsync(It.IsAny<PhotobookUser>(), It.IsAny<string>()))
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
                IsActive = false
            };
            await _context.AddAsync(user);
            await _context.SaveChangesAsync();
            var command = new RegisterUser.Command
            {
                Email = email,
                Password = "Password",
                Token = "Token"
            };

            var result = await _classUnderTest.Handle(command, default);

            Assert.False(result.Success);
            Assert.Equal(ErrorCodes.InvalidToken, result.Error.Code);
            Assert.Equal(ErrorMessages.InvalidToken, result.Error.Message);
            Assert.Equal(HttpStatusCode.BadRequest, result.Error.StatusCode);
        }

        [Fact]
        public async Task Handle_ShouldReturnsUnexpectedError_WhenAnExceptionIsThrown()
        {
            var email = "Email";
            var exception = new Exception("Message");
            _userManager.Setup(x => x.ConfirmEmailAsync(It.IsAny<PhotobookUser>(), It.IsAny<string>()))
                .Throws(exception);
            var user = new PhotobookUser
            {
                Email = email,
                IsActive = false
            };
            await _context.AddAsync(user);
            await _context.SaveChangesAsync();
            var command = new RegisterUser.Command
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
