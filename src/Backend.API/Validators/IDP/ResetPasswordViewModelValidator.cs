using Backend.API.IdentityServer.PasswordReset;
using Backend.API.Validators.Rules;
using FluentValidation;

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