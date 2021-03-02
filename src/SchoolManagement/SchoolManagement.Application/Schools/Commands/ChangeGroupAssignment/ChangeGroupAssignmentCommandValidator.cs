using FluentValidation;
using SharedKernel.Infrastructure.Extensions;

namespace SchoolManagement.Application.Schools.Commands.ChangeGroupAssignment
{
    internal sealed class ChangeGroupAssignmentCommandValidator : AbstractValidator<ChangeGroupAssignmentCommand>
    {
        public ChangeGroupAssignmentCommandValidator()
        {
            RuleFor(p => p.GroupId).GuidIdMustBeValid();
            RuleFor(p => p.StudentId).GuidIdMustBeValid();
            RuleFor(p => p.SchoolId).GuidIdMustBeValid();
        }
    }
}