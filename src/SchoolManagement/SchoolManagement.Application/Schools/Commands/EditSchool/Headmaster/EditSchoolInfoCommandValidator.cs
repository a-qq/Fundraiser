using FluentValidation;
using SchoolManagement.Application.Common.ValidationRules;
using SharedKernel.Infrastructure.Extensions;

namespace SchoolManagement.Application.Schools.Commands.EditSchool.Headmaster
{
    public class EditSchoolInfoCommandValidator : AbstractValidator<EditSchoolInfoCommand>
    {
        public EditSchoolInfoCommandValidator()
        {
            RuleFor(p => p.Description).DescriptionMustBeValid();
            RuleFor(p => p.GroupMembersLimit).GroupMembersLimitMustBeValid();
            RuleFor(p => p.SchoolId).GuidIdMustBeValid();
        }
    }
}