using FluentValidation;
using Backend.API.Validators.Rules;
using IdentityServerHost.Quickstart.UI;

namespace Backend.API.Validators.IDP
{
    public class ResetPasswordViewModelValidator : AbstractValidator<ResetPasswordViewModel>
    {
        public ResetPasswordViewModelValidator()
        {
            RuleFor(p => p.Password).PasswordMustBeValid();
            RuleFor(p => p.ConfirmPassword).Equal(p => p.Password).WithMessage("Passwords do not match!");
        }
    }
}
