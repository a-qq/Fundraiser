using FluentValidation;
using SharedKernel.Infrastructure.Extensions;

namespace SchoolManagement.Application.Schools.Commands.PromoteHeadmaster
{
    internal sealed class PromoteHeadmasterCommandValidator : AbstractValidator<PromoteHeadmasterCommand>
    {
        public PromoteHeadmasterCommandValidator()
        {
            RuleFor(c => c.TeacherId).GuidIdMustBeValid();
            RuleFor(c => c.SchoolId).GuidIdMustBeValid();
        }
    }
}