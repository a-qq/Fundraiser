using FluentValidation;
using SharedKernel.Infrastructure.Extensions;

namespace SchoolManagement.Application.Schools.Commands.ArchiveMember
{
    internal sealed class ArchiveMemberCommandValidator : AbstractValidator<ArchiveMemberCommand>
    {
        public ArchiveMemberCommandValidator()
        {
            RuleFor(p => p.MemberId).GuidIdMustBeValid();
            RuleFor(p => p.SchoolId).GuidIdMustBeValid();
        }
    }
}
