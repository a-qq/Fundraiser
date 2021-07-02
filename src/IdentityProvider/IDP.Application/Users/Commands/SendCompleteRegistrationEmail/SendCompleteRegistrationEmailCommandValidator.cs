using FluentValidation;
using IDP.Application.Common.Validation;

namespace IDP.Application.Users.Commands.SendCompleteRegistrationEmail
{
    internal sealed class SendCompleteRegistrationEmailCommandValidator : AbstractValidator<SendCompleteRegistrationEmailCommand>
    {
        public SendCompleteRegistrationEmailCommandValidator()
        {
            RuleFor(p => p.Subject).SubjectMustBeValid();
        }
    }
}
