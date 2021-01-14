﻿using FluentValidation;
using SchoolManagement.Core.SchoolAggregate.Groups;

namespace Fundraiser.API.Validators.Rules
{
    public static class GroupRules
    {
        public static IRuleBuilderInitial<T, int> NumberMustBeValid<T>(this IRuleBuilder<T, int> ruleBuilder)
        {
            return ruleBuilder.Custom((property, context) =>
            {
                var result = Number.Validate(property);
                if (result.IsFailure)
                    context.AddFailure(result.Error);
            });
        }

        public static IRuleBuilderInitial<T, string> SignMustBeValid<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Custom((property, context) =>
            {
                var result = Sign.Validate(property);
                if (result.IsFailure)
                    foreach (var error in result.Error.Errors)
                        context.AddFailure(error);
            });
        }
    }
}
