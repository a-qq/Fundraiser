using Backend.API.IdentityServer.Registration;
using Backend.API.Validators.Rules;
using FluentValidation;

namespace Backend.API.Validators.IDP
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