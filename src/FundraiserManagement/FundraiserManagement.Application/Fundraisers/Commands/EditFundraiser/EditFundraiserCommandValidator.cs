using FluentValidation;
using FundraiserManagement.Application.Common.ValidationRules;
using FMD = FundraiserManagement.Domain.FundraiserAggregate.Fundraisers;

namespace FundraiserManagement.Application.Fundraisers.Commands.EditFundraiser
{
    internal sealed class EditFundraiserCommandValidator : AbstractValidator<EditFundraiserCommand>
    {
        public EditFundraiserCommandValidator()
        {
            RuleFor(p => p.Name).NameMustBeValid();
            RuleFor(p => p.Description).DescriptionMustBeValid();
            RuleFor(p => p.Goal).GoalMustBeValid();
            RuleFor(p => p.SchoolId).NotEmpty();
            RuleFor(p => p.FundraiserId).NotEmpty();
            RuleFor(p => p.ManagerId).NotEmpty();
            RuleFor(p => p.Type).IsInEnum();
            RuleFor(p => p.Range).IsInEnum();
            RuleFor(p => p).Custom((property, context) =>
            {
                var result = FMD.Fundraiser.Validate(property.GroupId, property.Range, property.Type,
                    nameof(property.GroupId), nameof(property.Range), nameof(property.Type));
                foreach (var error in result.Error.Errors)
                    context.AddFailure(error);

            });

            When(p => p.GroupId != null, () =>
            {
                RuleFor(p => p.GroupId).NotEmpty();
            });

            //When(p => p.Range == FMD.Range.Intragroup, () =>
            //    RuleFor(p => p.GroupId).NotEmpty());

            //When(p => p.Type != FMD.Type.Normal, () => 
            //    RuleFor(p => p.GroupId).NotEmpty());
        }
    }
}