using FluentValidation;
using SharedKernel.Infrastructure.Extensions;

namespace SchoolManagement.Application.Schools.Commands.DisenrollStudentFromGroup
{
    internal sealed class DisenrollStudentFromGroupCommandValidator : AbstractValidator<DisenrollStudentFromGroupCommand>
    {
        public DisenrollStudentFromGroupCommandValidator()
        {
            RuleFor(p => p.GroupId).GuidIdMustBeValid();
            RuleFor(p => p.StudentId).GuidIdMustBeValid();
            RuleFor(p => p.SchoolId).GuidIdMustBeValid();
        }
    }
}
