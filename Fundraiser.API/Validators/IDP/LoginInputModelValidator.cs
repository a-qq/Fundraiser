using FluentValidation;
using Fundraiser.API.Validators.Rules;
using IdentityServerHost.Quickstart.UI;

namespace Fundraiser.API.Validators.IDP
{
    public class LoginInputModelValidator : AbstractValidator<LoginInputModel>
    {
        public LoginInputModelValidator()
        {
            RuleFor(p => p.Email).EmailMustBeValid();
            RuleFor(p => p.Password).NotNull().WithMessage("Password is required!").NotEmpty().WithMessage("Password is required!");
        }
    }
}
