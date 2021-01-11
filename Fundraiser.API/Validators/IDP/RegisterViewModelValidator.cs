using FluentValidation;
using Fundraiser.API.Validators.Rules;
using IdentityServerHost.Quickstart.UI;

namespace Fundraiser.API.Validators.IDP
{
    public class RegisterViewModelValidator : AbstractValidator<RegisterViewModel>
    {
        public RegisterViewModelValidator()
        {
            RuleFor(p => p.Password).PasswordMustBeValid();
            RuleFor(p => p.ConfirmPassword).Equal(p => p.Password).WithMessage("Passwords do not match!");
        }
    }
}
