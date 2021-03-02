using FluentValidation;
using SharedKernel.Infrastructure.Extensions;

namespace SchoolManagement.Application.Schools.Commands.PromoteTreasurer
{
    internal sealed class PromoteTreasurerCommandValidator : AbstractValidator<PromoteTreasurerCommand>
    {
        public PromoteTreasurerCommandValidator()
        {
            RuleFor(p => p.StudentId).GuidIdMustBeValid();
            RuleFor(p => p.GroupId).GuidIdMustBeValid();
            RuleFor(p => p.SchoolId).GuidIdMustBeValid();
        }
    }
}