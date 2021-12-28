using Microsoft.AspNetCore.Http;
using Moq;
using Photobook.Common.HandlersResponses;
using Photobook.Common.Identity;
using Photobook.Common.Models;
using Photobook.Common.Services.CurrentUser;
using Photobook.Common.Services.Files;
using Photobook.Data;
using Photobook.Logic.Features.UsersSelf;
using Photobook.Tests.Helpers;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Photobook.Tests.Logic.Features.UsersSelf
{
    public class UpdateProfilePictureHandlerTests
    {
        private readonly PhotobookDbContext _context;
        private readonly Mock<ICurrentUserService> _currentUserService;
        private readonly Mock<IFilesService> _profilePicturesService;
        private readonly Mock<IFormFile> _file;
        private readonly UpdateProfilePicture.Handler _classUnderTest;

        public UpdateProfilePictureHandlerTests()
        {
            _context = ContextHelper.CreateContext();
            _currentUserService = new Mock<ICurrentUserService>();
            _profilePicturesService = new Mock<IFilesService>();
            _file = new Mock<IFormFile>();
            _classUnderTest = new UpdateProfilePicture.Handler(_currentUserService.Object,
                _profilePicturesService.Object,
                _context);
        }

        [Fact]
        public async Task Handle_ShouldReturnsProfileResponse_WhenSucceed()
        {
            var newProfilePicture = "NewProfilePicture";
            var user = new PhotobookUser
            {
                Email = "Email",
                IsActive = true
            };
            var profileImage = new Image
            {
                Path = "ProfilePicture"
            };
            var profile = new Profile
            {
                User = user,
                FirstName = "FirstName",
                LastName = "LastName",
                Description = "Description",
                ProfileImage = profileImage
            };
            await _context.AddAsync(profile);
            await _context.SaveChangesAsync();
            _currentUserService.Setup(x => x.GetUserId())
                .Returns(user.Id);
            _profilePicturesService.Setup(x => x.SaveFile(It.IsAny<IFormFile>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(newProfilePicture);
            var command = new UpdateProfilePicture.Command
            {
                ProfilePicture = _file.Object
            };

            var result = await _classUnderTest.Handle(command, default);

            Assert.True(result.Success);
            var profileResponse = result.Value;
            Assert.Equal(profile.FirstName, profileResponse.FirstName);
            Assert.Equal(profile.LastName, profileResponse.LastName);
            Assert.Equal(profile.Description, profileResponse.Description);
            Assert.Equal(newProfilePicture, profileResponse.ProfilePicture);
        }

        [Fact]
        public async Task Handle_ShouldReturnsInvalidUserError_WhenProfileNotExists()
        {
            _currentUserService.Setup(x => x.GetUserId())
                .Returns(Guid.NewGuid());
            var command = new UpdateProfilePicture.Command
            {
                ProfilePicture = _file.Object
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
            var profileImage = new Image
            {
                Path = "ProfilePicture"
            };
            var profile = new Profile
            {
                User = user,
                FirstName = "FirstName",
                LastName = "LastName",
                Description = "Description",
                ProfileImage = profileImage
            };
            await _context.AddAsync(profile);
            await _context.SaveChangesAsync();
            _currentUserService.Setup(x => x.GetUserId())
                .Returns(user.Id);
            var command = new UpdateProfilePicture.Command
            {
                ProfilePicture = _file.Object
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
            var classUnderTest = new UpdateProfilePicture.Handler(_currentUserService.Object,
                _profilePicturesService.Object,
                mockContext.Object);
            var command = new UpdateProfilePicture.Command
            {
                ProfilePicture = _file.Object
            };

            var result = await classUnderTest.Handle(command, default);

            Assert.False(result.Success);
            Assert.Equal(ErrorCodes.UnexpectedError, result.Error.Code);
            Assert.Equal(exception.Message, result.Error.Message);
            Assert.Equal(HttpStatusCode.InternalServerError, result.Error.StatusCode);
        }
    }
}
