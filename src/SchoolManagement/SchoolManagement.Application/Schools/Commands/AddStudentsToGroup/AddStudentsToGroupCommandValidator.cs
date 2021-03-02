using FluentValidation;
using SharedKernel.Infrastructure.Extensions;

namespace SchoolManagement.Application.Schools.Commands.AddStudentsToGroup
{
    internal sealed class AddStudentsToGroupCommandValidator : AbstractValidator<AddStudentsToGroupCommand>
    {
        public AddStudentsToGroupCommandValidator()
        {
            RuleForEach(p => p.StudentIds).GuidIdMustBeValid();
            RuleFor(p => p.GroupId).GuidIdMustBeValid();
            RuleFor(p => p.SchoolId).GuidIdMustBeValid();
        }
    }
}