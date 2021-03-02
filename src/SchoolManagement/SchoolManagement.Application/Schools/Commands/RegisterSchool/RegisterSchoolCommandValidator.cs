using FluentValidation;
using SchoolManagement.Application.Common.ValidationRules;
using SharedKernel.Infrastructure.Extensions;

namespace SchoolManagement.Application.Schools.Commands.RegisterSchool
{
    internal sealed class RegisterSchoolCommandValidator : AbstractValidator<RegisterSchoolCommand>
    {
        public RegisterSchoolCommandValidator()
        {
            RuleFor(p => p.Name).NameMustBeValid();
            RuleFor(p => p.YearsOfEducation).YearsOfEducationMustBeValid();
            RuleFor(p => p.HeadmasterFirstName).FirstNameMustBeValid();
            RuleFor(p => p.HeadmasterLastName).LastNameMustBeValid();
            RuleFor(p => p.HeadmasterEmail).EmailMustBeValid();
            RuleFor(p => p.HeadmasterGender).GenderMustBeValid();
        }
    }
}
