using FluentValidation;
using SchoolManagement.Application.Common.ValidationRules;
using SchoolManagement.Application.Schools.Commands.CreateGroup;
using SharedKernel.Infrastructure.Extensions;

namespace Backend.API.Validators.Management
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
