using FluentValidation;
using SharedKernel.Infrastructure.Extensions;

namespace SchoolManagement.Application.Schools.Commands.AssignStudentToGroup
{
    internal sealed class AssignStudentToGroupCommandValidator : AbstractValidator<AssignStudentToGroupCommand>
    {
        public AssignStudentToGroupCommandValidator()
        {
            RuleFor(p => p.GroupId).GuidIdMustBeValid();
            RuleFor(p => p.StudentId).GuidIdMustBeValid();
            RuleFor(p => p.SchoolId).GuidIdMustBeValid();
        }
    }
}