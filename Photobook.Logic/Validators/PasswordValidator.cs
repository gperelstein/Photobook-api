using Microsoft.AspNetCore.Identity;
using Photobook.Models.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photobook.Logic.Validators
{
    public class PasswordValidator : IPasswordValidator<PhotobookUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<PhotobookUser> manager, PhotobookUser user, string password)
        {
            return Task.FromResult(ValidatePasswordRequirements(password));
        }

        private IdentityResult ValidatePasswordRequirements(string password)
        {
            var identityErrors = new List<IdentityError>();

            if (!password.Any(char.IsUpper))
            {
                identityErrors.Add(
                        new IdentityError
                        {
                            Code = "PasswordRequiresUpper",
                            Description = "Passwords must have at least one uppercase ('A'-'Z')."
                        }
                    );
            }

            if (!password.Any(char.IsLower))
            {
                identityErrors.Add(
                        new IdentityError
                        {
                            Code = "PasswordRequiresLower",
                            Description = "Passwords must have at least one lowercase ('a'-'z')."
                        }
                    );
            }

            if (!password.Any(char.IsNumber))
            {
                identityErrors.Add(
                        new IdentityError
                        {
                            Code = "PasswordRequiresDigit",
                            Description = "Passwords must have at least one digit ('0'-'9')."
                        }
                    );
            }

            var specialCharacters = @"~`!@#$%^&*()+=_-{}[]\|:;”'?/<>,.";
            if (password.IndexOfAny(specialCharacters.ToCharArray()) == -1)
            {
                identityErrors.Add(
                    new IdentityError
                    {
                        Code = "PasswordRequiresNonAlphanumeric",
                        Description = @"Passwords must have at least one non alphanumeric character. Allowed characters: ~`!@#$%^&*()+=_-{}[]\ | :;”'?/<>,."
                    }
                );
            }

            if (identityErrors.Count > 1)
            {
                return IdentityResult.Failed(identityErrors.ToArray());
            }

            return IdentityResult.Success;
        }
    }
}
