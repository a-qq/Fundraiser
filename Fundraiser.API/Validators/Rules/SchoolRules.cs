﻿using FluentValidation;
using SchoolManagement.Core.SchoolAggregate.Schools;

namespace Fundraiser.API.Validators.Rules
{
    public static class SchoolRules
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

        public static IRuleBuilderInitial<T, int?> GroupMembersLimitMustBeValid<T>(this IRuleBuilder<T, int?> ruleBuilder)
        {
            return ruleBuilder.Custom((property, context) =>
            {
                var result = GroupMembersLimit.Validate(property, context.PropertyName);
                if (result.IsFailure)
                    context.AddFailure(result.Error);
            });
        }
    }
}
