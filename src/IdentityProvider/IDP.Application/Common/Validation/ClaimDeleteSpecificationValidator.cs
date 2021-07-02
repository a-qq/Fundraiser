using FluentValidation;
using IDP.Application.Common.Models;

namespace IDP.Application.Common.Validation
{
    internal sealed class ClaimDeleteSpecificationValidator : AbstractValidator<ClaimDeleteSpecification>
    {
        public ClaimDeleteSpecificationValidator()
        {
            RuleFor(c => c.Type).NotEmpty();
            When(p => p.Value.HasValue,
                () => { RuleFor(p => p.Value.Value).NotEmpty().WithName(p=>nameof(p.Value)); });
        }
    }
}
