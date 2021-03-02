using FluentValidation;
using SharedKernel.Infrastructure.Extensions;

namespace SchoolManagement.Application.Schools.Commands.DivestFormTutor
{
    internal sealed class DivestFormTutorCommandValidator : AbstractValidator<DivestFormTutorCommand>
    {
        public DivestFormTutorCommandValidator()
        {
            RuleFor(p => p.GroupId).GuidIdMustBeValid();
            RuleFor(p => p.SchoolId).GuidIdMustBeValid();
        }
    }
}