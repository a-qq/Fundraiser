using FluentValidation;
using IDP.Application.Common.Validation;

namespace IDP.Application.Users.Commands.SendResetPasswordEmail
{
    internal sealed class SendResetPasswordEmailCommandValidator : AbstractValidator<SendResetPasswordEmailCommand>
    {
        public SendResetPasswordEmailCommandValidator()
        {
            RuleFor(p => p.Subject).SubjectMustBeValid();
        }
    }
}
