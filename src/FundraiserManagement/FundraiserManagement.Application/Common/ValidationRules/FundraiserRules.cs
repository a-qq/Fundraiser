using FluentValidation;
using FundraiserManagement.Domain.FundraiserAggregate.Fundraisers;

namespace FundraiserManagement.Application.Common.ValidationRules
{
    internal static class FundraiserRules
    {
        public static IRuleBuilderInitial<T, string> NameMustBeValid<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Custom((property, context) =>
            {
                var result = Name.Validate(property, context.PropertyName);
                if (result.IsFailure)
                    context.AddFailure(result.Error);
            });
        }

        public static IRuleBuilderInitial<T, string> DescriptionMustBeValid<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Custom((property, context) =>
            {
                var result = Description.Validate(property, context.PropertyName);
                if (result.IsFailure)
                    context.AddFailure(result.Error);
            });
        }

        public static IRuleBuilderInitial<T, decimal> GoalMustBeValid<T>(this IRuleBuilder<T, decimal> ruleBuilder)
        {
            return ruleBuilder.Custom((property, context) =>
            {
                var result = Goal.Validate(property, context.PropertyName);
                if (result.IsFailure)
                    context.AddFailure(result.Error);
            });
        }
    }
}
