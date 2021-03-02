using FluentValidation;
using SharedKernel.Infrastructure.Extensions;

namespace SchoolManagement.Application.Schools.Commands.DivestHeadmaster
{
    internal sealed class DivestHeadmasterCommandValidator : AbstractValidator<DivestHeadmasterCommand>
    {
        public DivestHeadmasterCommandValidator()
        {
            RuleFor(p => p.SchoolId).GuidIdMustBeValid();
        }
    }
}