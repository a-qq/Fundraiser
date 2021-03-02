using Backend.API.IdentityServer.Account;
using FluentValidation;
using SharedKernel.Infrastructure.Extensions;

namespace Backend.API.Validators.IDP
{
    public class LoginInputModelValidator : AbstractValidator<LoginInputModel>
    {
        public LoginInputModelValidator()
        {
            RuleFor(p => p.Email).EmailMustBeValid();
            RuleFor(p => p.Password).NotNull().WithMessage("Password is required!").NotEmpty()
                .WithMessage("Password is required!");
        }
    }
}