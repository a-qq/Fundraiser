using FluentValidation;
using SharedKernel.Infrastructure.Extensions;

namespace SchoolManagement.Application.Schools.Commands.AssignStudentsToGroup
{
    internal sealed class AssignStudentsToGroupCommandValidator : AbstractValidator<AssignStudentsToGroupCommand>
    {
        public AssignStudentsToGroupCommandValidator()
        {
            RuleForEach(p => p.StudentIds).GuidIdMustBeValid();
            RuleFor(p => p.GroupId).GuidIdMustBeValid();
            RuleFor(p => p.SchoolId).GuidIdMustBeValid();
        }
    }
}