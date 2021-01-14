using FluentValidation;
using Fundraiser.API.Validators.Rules;
using SchoolManagement.Data.Schools.CreateGroup;

namespace Fundraiser.API.Validators.Management
{
    public class CreateGroupRequestValidator : AbstractValidator<CreateGroupRequest>
    {
        public CreateGroupRequestValidator()
        {
            RuleFor(p => p.Number).NumberMustBeValid();
            RuleFor(p => p.Sign).SignMustBeValid();
        }
    }
}
