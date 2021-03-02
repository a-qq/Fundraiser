using FluentValidation;
using SharedKernel.Infrastructure.Extensions;

namespace SchoolManagement.Application.Schools.Commands.DivestTreasurer
{
    internal sealed class DivestTreasurerCommandValidator : AbstractValidator<DivestTreasurerCommand>
    {
        public DivestTreasurerCommandValidator()
        {
            RuleFor(p => p.GroupId).GuidIdMustBeValid();
            RuleFor(p => p.SchoolId).GuidIdMustBeValid();
        }
    }
}