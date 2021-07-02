using FluentValidation;
using IDP.Application.Common.Validation;

namespace IDP.Application.Users.Commands.UpdateClaims
{
    internal sealed class UpdateClaimsCommandValidator : AbstractValidator<UpdateClaimsCommand>
    {
        public UpdateClaimsCommandValidator()
        {
            RuleFor(p => p.Subject).SubjectMustBeValid();
            RuleFor(p => p.ClaimSpecifications).NotEmpty()
                .ForEach(p => p.SetValidator(new ClaimUpdateSpecificationValidator()));
        }
    }
}
