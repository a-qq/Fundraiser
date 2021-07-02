using FluentValidation;
using IDP.Application.Common.Validation;

namespace IDP.Application.Users.Commands.CompleteRegistration
{
    internal sealed class CompleteRegistrationCommandValidator : AbstractValidator<CompleteRegistrationCommand>
    {
        public CompleteRegistrationCommandValidator()
        {
            RuleFor(p => p.Password).PasswordMustBeValid();
            RuleFor(p => p.SecurityCode).SecurityCodeMustBeValid();
        }
    }
}
