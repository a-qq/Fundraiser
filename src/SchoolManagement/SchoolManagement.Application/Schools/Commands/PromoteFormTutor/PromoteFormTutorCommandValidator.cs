using FluentValidation;
using SharedKernel.Infrastructure.Extensions;

namespace SchoolManagement.Application.Schools.Commands.PromoteFormTutor
{
    public sealed class PromoteFormTutorCommandValidator : AbstractValidator<PromoteFormTutorCommand>
    {
        public PromoteFormTutorCommandValidator()
        {
            RuleFor(p => p.TeacherId).GuidIdMustBeValid();
            RuleFor(p => p.SchoolId).GuidIdMustBeValid();
            RuleFor(p => p.GroupId).GuidIdMustBeValid();
        }
    }
}