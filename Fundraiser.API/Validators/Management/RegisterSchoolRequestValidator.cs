using CSharpFunctionalExtensions;
using FluentValidation;
using Fundraiser.API.Validators.Rules;
using Fundraiser.SharedKernel.Utils;
using SchoolManagement.Core.SchoolAggregate.Schools;
using SchoolManagement.Core.SchoolAggregate.Members;
using SchoolManagement.Data.Schools.Commands.RegisterSchool;

namespace Fundraiser.API.Validators.Management
{
    public class RegisterSchoolRequestValidator : AbstractValidator<RegisterSchoolRequest>
    {
        public RegisterSchoolRequestValidator()
        {
            RuleFor(p => p.Name).NameMustBeValid();
            RuleFor(p => p.HeadmasterFirstName).FirstNameMustBeValid();
            RuleFor(p => p.HeadmasterLastName).LastNameMustBeValid();
            RuleFor(p => p.HeadmasterEmail).EmailMustBeValid();
            RuleFor(p => p.HeadmasterGender).GenderMustBeValid();
        }
    }
}
