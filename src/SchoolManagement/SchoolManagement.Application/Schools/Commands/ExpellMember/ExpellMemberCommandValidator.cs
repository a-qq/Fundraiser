using FluentValidation;
using SharedKernel.Infrastructure.Extensions;

namespace SchoolManagement.Application.Schools.Commands.ExpellMember
{
    internal sealed class ExpellMemberCommandValidator : AbstractValidator<ExpellMemberCommand>
    {
        public ExpellMemberCommandValidator()
        {
            RuleFor(p => p.MemberId).GuidIdMustBeValid();
            RuleFor(p => p.SchoolId).GuidIdMustBeValid();
        }
    }
}