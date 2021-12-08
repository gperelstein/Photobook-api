using FluentValidation.TestHelper;
using Photobook.Logic.Features.Users;
using Xunit;

namespace Photobook.Tests.Logic.Features.Users
{
    public class CreateUserValidatorTests
    {
        private readonly CreateUser.Validator _classUnderTest;

        public CreateUserValidatorTests()
        {
            _classUnderTest = new CreateUser.Validator();
        }

        [Fact]
        public void Validate_ShouldNotReportError_WhenCommandIsValid()
        {
            var command = new CreateUser.Command
            {
                FirstName = "FirstName",
                LastName = "LastName",
                Email = "email@test.com"
            };

            var result = _classUnderTest.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_ShouldReportErrorForEmail_WhenEmailIsNotValid()
        {
            var command = new CreateUser.Command
            {
                FirstName = "FirstName",
                LastName = "LastName",
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
            var command = new CreateUser.Command
            {
                FirstName = "FirstName",
                LastName = "LastName"
            };

            var result = _classUnderTest.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Email);
            var error = Assert.Single(result.Errors);
            Assert.Equal("NotEmptyValidator", error.ErrorCode);
        }

        [Fact]
        public void Validate_ShouldReportErrorForFirstName_WhenFirstNameIsNull()
        {
            var command = new CreateUser.Command
            {
                LastName = "LastName",
                Email = "email@test.com"
            };

            var result = _classUnderTest.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.FirstName);
            var error = Assert.Single(result.Errors);
            Assert.Equal("NotEmptyValidator", error.ErrorCode);
        }

        [Fact]
        public void Validate_ShouldReportErrorForLastName_WhenLastNameIsNull()
        {
            var command = new CreateUser.Command
            {
                FirstName = "FirstName",
                Email = "email@test.com"
            };

            var result = _classUnderTest.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.LastName);
            var error = Assert.Single(result.Errors);
            Assert.Equal("NotEmptyValidator", error.ErrorCode);
        }

        [Fact]
        public void Validate_ShouldReportErrorForEmail_WhenEmailIsEmpty()
        {
            var command = new CreateUser.Command
            {
                FirstName = "FirstName",
                LastName = "LastName",
                Email = ""
            };

            var result = _classUnderTest.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Email);
            var error = Assert.Single(result.Errors);
            Assert.Equal("NotEmptyValidator", error.ErrorCode);
        }

        [Fact]
        public void Validate_ShouldReportErrorForFirstName_WhenFirstNameIsEmpty()
        {
            var command = new CreateUser.Command
            {
                FirstName = "",
                LastName = "LastName",
                Email = "email@test.com"
            };

            var result = _classUnderTest.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.FirstName);
            var error = Assert.Single(result.Errors);
            Assert.Equal("NotEmptyValidator", error.ErrorCode);
        }

        [Fact]
        public void Validate_ShouldReportErrorForLastName_WhenLastNameIsEmpty()
        {
            var command = new CreateUser.Command
            {
                FirstName = "FirstName",
                LastName = "",
                Email = "email@test.com"
            };

            var result = _classUnderTest.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.LastName);
            var error = Assert.Single(result.Errors);
            Assert.Equal("NotEmptyValidator", error.ErrorCode);
        }
    }
}
