using FluentValidation;
using SharedKernel.Infrastructure.Extensions;

namespace SchoolManagement.Application.Schools.Commands.ExpelMember
{
    internal sealed class ExpelMemberCommandValidator : AbstractValidator<ExpelMemberCommand>
    {
        public ExpelMemberCommandValidator()
        {
            RuleFor(p => p.MemberId).GuidIdMustBeValid();
            RuleFor(p => p.SchoolId).GuidIdMustBeValid();
        }
    }
}