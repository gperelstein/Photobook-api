using Moq;
using Photobook.Common.HandlersResponses;
using Photobook.Common.Identity;
using Photobook.Common.Models;
using Photobook.Common.Services.CurrentUser;
using Photobook.Data;
using Photobook.Logic.Features.UsersSelf;
using Photobook.Tests.Helpers;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Photobook.Tests.Logic.Features.UsersSelf
{
    public class UpdateProfileHandlerTests
    {
        private readonly PhotobookDbContext _context;
        private readonly Mock<ICurrentUserService> _currentUserService;
        private readonly UpdateProfile.Handler _classUnderTest;

        public UpdateProfileHandlerTests()
        {
            _context = ContextHelper.CreateContext();
            _currentUserService = new Mock<ICurrentUserService>();
            _classUnderTest = new UpdateProfile.Handler(_currentUserService.Object, _context);
        }

        [Fact]
        public async Task Handle_ShouldReturnsProfileResponse_WhenSucceed()
        {
            var user = new PhotobookUser
            {
                Email = "Email",
                IsActive = true
            };
            var profile = new Profile
            {
                User = user,
                FirstName = "FirstName",
                LastName = "LastName",
                Description = "Description",
                ProfilePicture = "ProfilePicture"
            };
            await _context.AddAsync(profile);
            await _context.SaveChangesAsync();
            _currentUserService.Setup(x => x.GetUserId())
                .Returns(user.Id);
            var command = new UpdateProfile.Command
            {
                FirstName = "NewFirstName",
                LastName = "NewLastName",
                Description = "NewDescription"
            };

            var result = await _classUnderTest.Handle(command, default);

            Assert.True(result.Success);
            var profileResponse = result.Value;
            Assert.Equal(command.FirstName, profileResponse.FirstName);
            Assert.Equal(command.LastName, profileResponse.LastName);
            Assert.Equal(command.Description, profileResponse.Description);
            Assert.Equal(profile.ProfilePicture, profileResponse.ProfilePicture);
        }

        [Fact]
        public async Task Handle_ShouldReturnsInvalidUserError_WhenProfileNotExists()
        {
            _currentUserService.Setup(x => x.GetUserId())
                .Returns(Guid.NewGuid());
            var command = new UpdateProfile.Command
            {
                FirstName = "NewFirstName",
                LastName = "NewLastName",
                Description = "NewDescription"
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
            var profile = new Profile
            {
                User = user,
                FirstName = "FirstName",
                LastName = "LastName",
                Description = "Description",
                ProfilePicture = "ProfilePicture"
            };
            await _context.AddAsync(profile);
            await _context.SaveChangesAsync();
            _currentUserService.Setup(x => x.GetUserId())
                .Returns(user.Id);
            var command = new UpdateProfile.Command
            {
                FirstName = "NewFirstName",
                LastName = "NewLastName",
                Description = "NewDescription"
            };

            var result = await _classUnderTest.Handle(command, default);

            Assert.False(result.Success);
            Assert.Equal(ErrorCodes.UserNotActive, result.Error.Code);
            Assert.Equal(ErrorMessages.UserNotActive, result.Error.Message);
            Assert.Equal(HttpStatusCode.BadRequest, result.Error.StatusCode);
        }

        [Fact]
        public async Task Handle_ShouldReturnsUnexpectedErrorError_WhenAnExceptionIsThrown()
        {
            var exception = new Exception("Message");
            var mockContext = ContextHelper.CreateMockContext();
            mockContext.Setup(x => x.Profiles)
                .Throws(exception);
            _currentUserService.Setup(x => x.GetUserId())
                .Returns(Guid.NewGuid());
            var classUnderTest = new UpdateProfile.Handler(_currentUserService.Object, mockContext.Object);
            var command = new UpdateProfile.Command
            {
                FirstName = "NewFirstName",
                LastName = "NewLastName",
                Description = "NewDescription"
            };

            var result = await classUnderTest.Handle(command, default);

            Assert.False(result.Success);
            Assert.Equal(ErrorCodes.UnexpectedError, result.Error.Code);
            Assert.Equal(exception.Message, result.Error.Message);
            Assert.Equal(HttpStatusCode.InternalServerError, result.Error.StatusCode);
        }
    }
}
