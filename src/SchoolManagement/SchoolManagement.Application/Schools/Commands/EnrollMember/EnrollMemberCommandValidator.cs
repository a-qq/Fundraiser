using FluentValidation;
using SchoolManagement.Application.Common.ValidationRules;
using SharedKernel.Infrastructure.Extensions;

namespace SchoolManagement.Application.Schools.Commands.EnrollMember
{
    internal sealed class EnrollMemberCommandValidator : AbstractValidator<EnrollMemberCommand>
    {
        public EnrollMemberCommandValidator()
        {
            RuleFor(p => p.FirstName).FirstNameMustBeValid();
            RuleFor(p => p.LastName).LastNameMustBeValid();
            RuleFor(p => p.Email).EmailMustBeValid();
            RuleFor(p => p.Role).RoleMustBeValid();
            RuleFor(p => p.Gender).GenderMustBeValid();
            RuleFor(p => p.SchoolId).GuidIdMustBeValid();
        }
    }
}