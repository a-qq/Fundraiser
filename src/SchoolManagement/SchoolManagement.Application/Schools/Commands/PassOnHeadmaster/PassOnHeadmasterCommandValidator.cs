using FluentValidation;
using SharedKernel.Infrastructure.Extensions;

namespace SchoolManagement.Application.Schools.Commands.PassOnHeadmaster
{
    internal sealed class PassOnHeadmasterCommandValidator : AbstractValidator<PassOnHeadmasterCommand>
    {
        public PassOnHeadmasterCommandValidator()
        {
            RuleFor(c => c.TeacherId).GuidIdMustBeValid();
            RuleFor(c => c.SchoolId).GuidIdMustBeValid();
        }
    }
}
