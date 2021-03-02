using FluentValidation;
using SharedKernel.Infrastructure.Extensions;

namespace SchoolManagement.Application.Schools.Commands.Graduate
{
    internal sealed class GraduateCommandValidator : AbstractValidator<GraduateCommand>
    {
        public GraduateCommandValidator()
        {
            RuleFor(p => p.SchoolId).GuidIdMustBeValid();
        }
    }
}