using FluentValidation;
using IDP.Application.Common.Validation;

namespace IDP.Application.Users.Commands.DeactivateUser
{
    internal sealed class DeactivateUserCommandValidator : AbstractValidator<DeactivateUserCommand>
    {
        public DeactivateUserCommandValidator()
        {
            RuleFor(p => p.Subject).SubjectMustBeValid();
            When(p => !(p.ClaimSpecifications is null), 
                () => RuleForEach(x => x.ClaimSpecifications)
                        .NotNull()
                        .SetValidator(new ClaimDeleteSpecificationValidator()));
        }
    }
}
