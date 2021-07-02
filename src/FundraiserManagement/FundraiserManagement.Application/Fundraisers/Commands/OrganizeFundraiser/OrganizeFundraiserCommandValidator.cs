using FluentValidation;
using FundraiserManagement.Application.Common.ValidationRules;
using FMD = FundraiserManagement.Domain.FundraiserAggregate.Fundraisers;

namespace FundraiserManagement.Application.Fundraisers.Commands.OrganizeFundraiser
{
    internal sealed class OrganizeFundraiserCommandValidator : AbstractValidator<OrganizeFundraiserTypeCommand>
    {
        public OrganizeFundraiserCommandValidator()
        {
            RuleFor(p => p.Name).NameMustBeValid();
            RuleFor(p => p.Description).DescriptionMustBeValid();
            RuleFor(p => p.Goal).GoalMustBeValid();
            RuleFor(p => p.SchoolId).NotEmpty();
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
                //RuleFor(p => p.Range).Must(x => x == FMD.Range.Intragroup || x == FMD.Range.Intergroup);
            });

            //When(p => p.Range == FMD.Range.Intragroup || p.Range == FMD.Range.Intergroup, () =>
            //{
            //    RuleFor(p => p.GroupId).NotEmpty();
            //});

            //When(p => p.Type != FMD.Type.Normal, () =>
            //{
            //    RuleFor(p => p.GroupId).NotEmpty();
            //    RuleFor(p => p.Range).Must(x => x == FMD.Range.Intragroup);
            //});
        }
    }
}