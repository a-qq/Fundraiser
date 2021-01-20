using FluentValidation;
using Fundraiser.API.Validators.Rules;
using SchoolManagement.Data.Schools.EditSchool.Headmaster;

namespace Fundraiser.API.Validators.Management
{
    public class EditSchoolInfoRequestValidator : AbstractValidator<EditSchoolInfoRequest>
    {
        public EditSchoolInfoRequestValidator()
        {
            RuleFor(p => p.Description).DescriptionMustBeValid();
            RuleFor(p => p.MaxNumberOfMembersInGroup).GroupMembersLimitMustBeValid();
        }
    }
}
