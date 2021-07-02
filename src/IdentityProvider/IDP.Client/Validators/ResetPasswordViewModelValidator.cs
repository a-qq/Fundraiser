using FluentValidation;
using IDP.Client.Controllers.PasswordReset;

namespace IDP.Client.Validators
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