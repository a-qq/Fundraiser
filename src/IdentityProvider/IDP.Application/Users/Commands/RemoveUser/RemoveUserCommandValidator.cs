using FluentValidation;
using IDP.Application.Common.Validation;

namespace IDP.Application.Users.Commands.RemoveUser
{
    internal sealed class RemoveUserCommandValidator : AbstractValidator<RemoveUserCommand>
    {
        public RemoveUserCommandValidator()
        {
            RuleFor(p => p.Subject).SubjectMustBeValid();
        }
    }
}
