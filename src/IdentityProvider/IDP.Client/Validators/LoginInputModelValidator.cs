using FluentValidation;
using IDP.Client.Controllers.Account;
using SharedKernel.Infrastructure.Extensions;

namespace IDP.Client.Validators
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