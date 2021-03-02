using FluentValidation;
using SchoolManagement.Application.Common.ValidationRules;
using SharedKernel.Infrastructure.Extensions;

namespace SchoolManagement.Application.Schools.Commands.EditSchool.Admin
{
    internal class EditSchoolCommandValidator : AbstractValidator<EditSchoolCommand>
    {
        public EditSchoolCommandValidator()
        {
            RuleFor(p => p.Name).NameMustBeValid();
            RuleFor(p => p.Description).DescriptionMustBeValid();
            RuleFor(p => p.GroupMembersLimit).GroupMembersLimitMustBeValid();
            RuleFor(p => p.SchoolId).GuidIdMustBeValid();
        }
    }
}