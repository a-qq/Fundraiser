using FluentValidation;
using SchoolManagement.Data.Schools.PromoteTreasurer;

namespace Fundraiser.API.Validators.Management
{
    internal sealed class PromoteTreasurerRequestValidator : AbstractValidator<PromoteTreasurerRequest>
    {
        public PromoteTreasurerRequestValidator()
        {
            RuleFor(p => p.StudentId).NotEmpty().WithMessage("{PropertyName} is required!");
        }
    }
}
