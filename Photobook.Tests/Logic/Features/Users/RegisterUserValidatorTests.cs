using FluentValidation.TestHelper;
using Photobook.Logic.Features.Users;
using Xunit;

namespace Photobook.Tests.Logic.Features.Users
{
    public class RegisterUserValidatorTests
    {
        private readonly RegisterUser.Validator _classUnderTest;

        public RegisterUserValidatorTests()
        {
            _classUnderTest = new RegisterUser.Validator();
        }

        [Fact]
        public void Validate_ShouldNotReportError_WhenCommandIsValid()
        {
            var command = new RegisterUser.Command
            {
                Email = "email@test.com",
                Password = "Test*1234",
                Token = "Token"
            };

            var result = _classUnderTest.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_ShouldReportErrorForEmail_WhenEmailIsNotValid()
        {
            var command = new RegisterUser.Command
            {
                Email = "email",
                Password = "Test*1234",
                Token = "Token"
            };

            var result = _classUnderTest.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Email);
            var error = Assert.Single(result.Errors);
            Assert.Equal("EmailValidator", error.ErrorCode);
        }

        [Fact]
        public void Validate_ShouldReportErrorForEmail_WhenEmailIsNull()
        {
            var command = new RegisterUser.Command
            {
                Password = "Test*1234",
                Token = "Token"
            };

            var result = _classUnderTest.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Email);
            var error = Assert.Single(result.Errors);
            Assert.Equal("NotEmptyValidator", error.ErrorCode);
        }

        [Fact]
        public void Validate_ShouldReportErrorForEmail_WhenEmailIsEmpty()
        {
            var command = new RegisterUser.Command
            {
                Email = "",
                Password = "Test*1234",
                Token = "Token"
            };

            var result = _classUnderTest.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Email);
            var error = Assert.Single(result.Errors);
            Assert.Equal("NotEmptyValidator", error.ErrorCode);
        }

        [Fact]
        public void Validate_ShouldReportErrorForPassword_WhenPasswordIsNull()
        {
            var command = new RegisterUser.Command
            {
                Email = "email@test.com",
                Token = "Token"
            };

            var result = _classUnderTest.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Password);
            var error = Assert.Single(result.Errors);
            Assert.Equal("NotEmptyValidator", error.ErrorCode);
        }

        [Fact]
        public void Validate_ShouldReportErrorForPassword_WhenPasswordIsEmpty()
        {
            var command = new RegisterUser.Command
            {
                Email = "email@test.com",
                Password = "",
                Token = "Token"
            };

            var result = _classUnderTest.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Password);
            var error = Assert.Single(result.Errors);
            Assert.Equal("NotEmptyValidator", error.ErrorCode);
        }

        [Fact]
        public void Validate_ShouldReportErrorForPassword_WhenPasswordIsNotValid()
        {
            var command = new RegisterUser.Command
            {
                Email = "email@test.com",
                Password = "wrongPassword",
                Token = "Token"
            };

            var result = _classUnderTest.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Password);
            var error = Assert.Single(result.Errors);
            Assert.Equal("RegularExpressionValidator", error.ErrorCode);
        }

        [Fact]
        public void Validate_ShouldReportErrorForToken_WhenTokenIsNull()
        {
            var command = new RegisterUser.Command
            {
                Email = "email@test.com",
                Password = "Test*1234"
            };

            var result = _classUnderTest.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Token);
            var error = Assert.Single(result.Errors);
            Assert.Equal("NotEmptyValidator", error.ErrorCode);
        }

        [Fact]
        public void Validate_ShouldReportErrorForToken_WhenTokenIsEmpty()
        {
            var command = new RegisterUser.Command
            {
                Email = "email@test.com",
                Password = "Test*1234",
                Token = ""
            };

            var result = _classUnderTest.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Token);
            var error = Assert.Single(result.Errors);
            Assert.Equal("NotEmptyValidator", error.ErrorCode);
        }
    }
}
