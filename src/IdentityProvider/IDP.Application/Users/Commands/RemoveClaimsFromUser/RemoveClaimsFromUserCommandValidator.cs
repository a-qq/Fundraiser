using FluentValidation;
using IDP.Application.Common.Validation;

namespace IDP.Application.Users.Commands.RemoveClaimsFromUser
{
    internal sealed class RemoveClaimsFromUserCommandValidator : AbstractValidator<RemoveClaimsFromUserCommand>
    {
        public RemoveClaimsFromUserCommandValidator()
        {
            RuleFor(p => p.Subject).SubjectMustBeValid();
            RuleFor(p => p.ClaimSpecifications).NotEmpty()
                .ForEach(p => p.SetValidator(new ClaimDeleteSpecificationValidator()));
        }
    }
}
