using FluentValidation;
using Fundraiser.API.Validators.Rules;
using SchoolManagement.Data.Schools.EditSchool.Admin;

namespace Fundraiser.API.Validators.Management
{
    public class EditSchoolRequestValidator : AbstractValidator<EditSchoolRequest>
    {
        public EditSchoolRequestValidator()
        {
            RuleFor(p => p.Name).NameMustBeValid();
            RuleFor(p => p.Description).DescriptionMustBeValid();
            RuleFor(p => p.MaxNumberOfMembersInGroup).GroupMembersLimitMustBeValid(); 
        }
    }
}
