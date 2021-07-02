using FluentValidation;
using SharedKernel.Infrastructure.Extensions;

namespace IDP.Application.Users.Commands.Login
{
    internal sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(p => p.Email).EmailMustBeValid();
            RuleFor(p => p.Password).NotEmpty();
        }
    }
}
