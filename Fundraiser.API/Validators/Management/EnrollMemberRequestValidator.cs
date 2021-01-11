using FluentValidation;
using Fundraiser.API.Validators.Rules;
using SchoolManagement.Data.Schools.EnrollMember;

namespace Fundraiser.API.Validators.Management
{
    public class EnrollMemberRequestValidator : AbstractValidator<EnrollMemberRequest>
    {
        public EnrollMemberRequestValidator()
        {
            RuleFor(p => p.FirstName).FirstNameMustBeValid();
            RuleFor(p => p.LastName).LastNameMustBeValid();
            RuleFor(p => p.Email).EmailMustBeValid();
            RuleFor(p => p.Role).RoleMustBeValid();
            RuleFor(p => p.Gender).GenderMustBeValid();
        }
    }
}
