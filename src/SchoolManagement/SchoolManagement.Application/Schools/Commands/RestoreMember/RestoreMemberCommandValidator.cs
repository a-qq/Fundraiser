using FluentValidation;

namespace SchoolManagement.Application.Schools.Commands.RestoreMember
{
    internal sealed class RestoreMemberCommandValidator : AbstractValidator<RestoreMemberCommand>
    {
        public RestoreMemberCommandValidator()
        {
            RuleFor(c => c.MemberId).NotEmpty().WithMessage("{PropertyName} cannot be empty ('{PropertyValue}')!");
            RuleFor(c => c.SchoolId).NotEmpty().WithMessage("{PropertyName} cannot be empty ('{PropertyValue}')!");
        }
    }
}