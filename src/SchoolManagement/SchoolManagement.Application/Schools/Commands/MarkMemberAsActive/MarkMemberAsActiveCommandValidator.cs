using FluentValidation;
using System;

namespace SchoolManagement.Application.Schools.Commands.MarkMemberAsActive
{
    internal sealed class MarkMemberAsActiveCommandValidator : AbstractValidator<MarkMemberAsActiveCommand>
    {
        public MarkMemberAsActiveCommandValidator()
        {
            RuleFor(p => p.Subject)
                .Must(p => Guid.TryParse(p, out _)).WithMessage("{PropertyName} must be in Guid format!");
        }
    }
}
