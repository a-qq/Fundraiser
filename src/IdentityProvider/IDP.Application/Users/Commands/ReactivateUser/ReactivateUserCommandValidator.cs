using FluentValidation;
using IDP.Application.Common.Validation;

namespace IDP.Application.Users.Commands.ReactivateUser
{
    internal sealed class ReactivateUserCommandValidator : AbstractValidator<ReactivateUserCommand>
    {
        public ReactivateUserCommandValidator()
        {
            RuleFor(p => p.Subject).SubjectMustBeValid();
        }
    }
}
