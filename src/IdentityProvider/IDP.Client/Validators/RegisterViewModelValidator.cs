using FluentValidation;
using IDP.Client.Controllers.Registration;

namespace IDP.Client.Validators
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