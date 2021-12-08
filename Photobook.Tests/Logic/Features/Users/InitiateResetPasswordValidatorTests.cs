using FluentValidation.TestHelper;
using Photobook.Logic.Features.Users;
using Xunit;

namespace Photobook.Tests.Logic.Features.Users
{
    public class InitiateResetPasswordValidatorTests
    {
        private readonly InitiatePasswordReset.Validator _classUnderTest;

        public InitiateResetPasswordValidatorTests()
        {
            _classUnderTest = new InitiatePasswordReset.Validator();
        }

        [Fact]
        public void Validate_ShouldNotReportError_WhenCommandIsValid()
        {
            var command = new InitiatePasswordReset.Command
            {
                Email = "email@test.com"
            };

            var result = _classUnderTest.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_ShouldReportErrorForEmail_WhenEmailIsNotValid()
        {
            var command = new InitiatePasswordReset.Command
            {
                Email = "email"
            };

            var result = _classUnderTest.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Email);
            var error = Assert.Single(result.Errors);
            Assert.Equal("EmailValidator", error.ErrorCode);
        }

        [Fact]
        public void Validate_ShouldReportErrorForEmail_WhenEmailIsNull()
        {
            var command = new InitiatePasswordReset.Command();

            var result = _classUnderTest.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Email);
            var error = Assert.Single(result.Errors);
            Assert.Equal("NotEmptyValidator", error.ErrorCode);
        }

        [Fact]
        public void Validate_ShouldReportErrorForEmail_WhenEmailIsEmpty()
        {
            var command = new InitiatePasswordReset.Command
            {
                Email = ""
            };

            var result = _classUnderTest.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Email);
            var error = Assert.Single(result.Errors);
            Assert.Equal("NotEmptyValidator", error.ErrorCode);
        }
    }
}
