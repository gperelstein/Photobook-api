using FluentValidation.TestHelper;
using Photobook.Logic.Features.UsersSelf;
using Xunit;

namespace Photobook.Tests.Logic.Features.UsersSelf
{
    public class UpdateProfileValidatorTests
    {
        private readonly UpdateProfile.Validator _classUnderTest;

        public UpdateProfileValidatorTests()
        {
            _classUnderTest = new UpdateProfile.Validator();
        }

        [Fact]
        public void Validate_ShouldNotReportError_WhenCommandIsValid()
        {
            var command = new UpdateProfile.Command
            {
                FirstName = "FirstName",
                LastName = "LastName"
            };

            var result = _classUnderTest.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_ShouldReportErrorForFirstName_WhenFirstNameIsNull()
        {
            var command = new UpdateProfile.Command
            {
                LastName = "LastName"
            };

            var result = _classUnderTest.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.FirstName);
            var error = Assert.Single(result.Errors);
            Assert.Equal("NotEmptyValidator", error.ErrorCode);
        }

        [Fact]
        public void Validate_ShouldReportErrorForLastName_WhenLastNameIsNull()
        {
            var command = new UpdateProfile.Command
            {
                FirstName = "FirstName"
            };

            var result = _classUnderTest.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.LastName);
            var error = Assert.Single(result.Errors);
            Assert.Equal("NotEmptyValidator", error.ErrorCode);
        }

        [Fact]
        public void Validate_ShouldReportErrorForFirstName_WhenFirstNameIsEmpty()
        {
            var command = new UpdateProfile.Command
            {
                FirstName = "",
                LastName = "LastName"
            };

            var result = _classUnderTest.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.FirstName);
            var error = Assert.Single(result.Errors);
            Assert.Equal("NotEmptyValidator", error.ErrorCode);
        }

        [Fact]
        public void Validate_ShouldReportErrorForLastName_WhenLastNameIsEmpty()
        {
            var command = new UpdateProfile.Command
            {
                FirstName = "FirstName",
                LastName = ""
            };

            var result = _classUnderTest.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.LastName);
            var error = Assert.Single(result.Errors);
            Assert.Equal("NotEmptyValidator", error.ErrorCode);
        }
    }
}
