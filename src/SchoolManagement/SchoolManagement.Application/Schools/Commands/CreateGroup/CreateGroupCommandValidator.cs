using FluentValidation;
using SchoolManagement.Application.Common.ValidationRules;
using SharedKernel.Infrastructure.Extensions;

namespace SchoolManagement.Application.Schools.Commands.CreateGroup
{
    internal class CreateGroupCommandValidator : AbstractValidator<CreateGroupCommand>
    {
        public CreateGroupCommandValidator()
        {
            RuleFor(p => p.Number).NumberMustBeValid();
            RuleFor(p => p.Sign).SignMustBeValid();
            RuleFor(p => p.SchoolId).GuidIdMustBeValid();
        }
    }
}