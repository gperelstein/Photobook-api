using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http;
using Moq;
using Photobook.Logic.Features.UsersSelf;
using Xunit;

namespace Photobook.Tests.Logic.Features.UsersSelf
{
    public class UpdateProfilePictureValidatorTests
    {
        private readonly Mock<IFormFile> _file;
        private readonly UpdateProfilePicture.Validator _classUnderTest;

        public UpdateProfilePictureValidatorTests()
        {
            _file = new Mock<IFormFile>();
            _classUnderTest = new UpdateProfilePicture.Validator();
        }

        [Fact]
        public void Validate_ShouldNotReportError_WhenFileExtensionIsValid()
        {
            _file.Setup(x => x.FileName)
                .Returns("file.jpg");
            var command = new UpdateProfilePicture.Command
            {
                ProfilePicture = _file.Object
            };

            var result = _classUnderTest.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_ShouldReportErrorProfilePicture_WhenFileExtensionIsNotValid()
        {
            _file.Setup(x => x.FileName)
                .Returns("file");
            var command = new UpdateProfilePicture.Command
            {
                ProfilePicture = _file.Object
            };

            var result = _classUnderTest.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.ProfilePicture);
            var error = Assert.Single(result.Errors);
            Assert.Equal("PredicateValidator", error.ErrorCode);
            Assert.Equal("File type not supported", error.ErrorMessage);
        }
    }
}
