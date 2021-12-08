using FluentValidation.TestHelper;
using Photobook.Logic.Features.UsersSelf;
using Xunit;

namespace Photobook.Tests.Logic.Features.UsersSelf
{
    public class ChangePasswordValidatiorTests
    {
        private readonly ChangePassword.Validator _classUnderTest;

        public ChangePasswordValidatiorTests()
        {
            _classUnderTest = new ChangePassword.Validator();
        }

        [Fact]
        public void Validate_ShouldNotReportError_WhenCommandIsValid()
        {
            var command = new ChangePassword.Command
            {
                CurrentPassword = "CurrentPassword",
                NewPassword = "Test*1234"
            };

            var result = _classUnderTest.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_ShouldReportErrorForCurrentPassword_WhenCurrentPasswordIsNull()
        {
            var command = new ChangePassword.Command
            {
                NewPassword = "Test*1234"
            };

            var result = _classUnderTest.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.CurrentPassword);
            var error = Assert.Single(result.Errors);
            Assert.Equal("NotEmptyValidator", error.ErrorCode);
        }

        [Fact]
        public void Validate_ShouldReportErrorForCurrentPassword_WhenCurrentPasswordIsEmpty()
        {
            var command = new ChangePassword.Command
            {
                CurrentPassword = "",
                NewPassword = "Test*1234"
            };

            var result = _classUnderTest.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.CurrentPassword);
            var error = Assert.Single(result.Errors);
            Assert.Equal("NotEmptyValidator", error.ErrorCode);
        }

        [Fact]
        public void Validate_ShouldReportErrorForNewPassword_WhenNewPasswordIsNotValid()
        {
            var command = new ChangePassword.Command
            {
                CurrentPassword = "CurrentPassword",
                NewPassword = "WrongPassword"
            };

            var result = _classUnderTest.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.NewPassword);
            var error = Assert.Single(result.Errors);
            Assert.Equal("RegularExpressionValidator", error.ErrorCode);
        }

        [Fact]
        public void Validate_ShouldReportErrorForNewPassword_WhenNewPasswordIsNull()
        {
            var command = new ChangePassword.Command
            {
                CurrentPassword = "CurrentPassword"
            };

            var result = _classUnderTest.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.NewPassword);
            var error = Assert.Single(result.Errors);
            Assert.Equal("NotEmptyValidator", error.ErrorCode);
        }

        [Fact]
        public void Validate_ShouldReportErrorForNewPassword_WhenNewPasswordIsEmpty()
        {
            var command = new ChangePassword.Command
            {
                CurrentPassword = "CurrentPassword",
                NewPassword = ""
            };

            var result = _classUnderTest.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.NewPassword);
            var error = Assert.Single(result.Errors);
            Assert.Equal("NotEmptyValidator", error.ErrorCode);
        }
    }
}
