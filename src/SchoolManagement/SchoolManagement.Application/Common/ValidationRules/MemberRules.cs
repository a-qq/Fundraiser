﻿using FluentValidation;
using SchoolManagement.Domain.SchoolAggregate.Members;

namespace SchoolManagement.Application.Common.ValidationRules
{
    internal static class MemberRules
    {
        public static IRuleBuilderInitial<T, string> FirstNameMustBeValid<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Custom((fistName, context) =>
            {
                var result = FirstName.Validate(fistName, context.PropertyName);
                if (result.IsFailure)
                    foreach (var error in result.Error.Errors)
                        context.AddFailure(error);
            });
        }

        public static IRuleBuilderInitial<T, string> LastNameMustBeValid<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Custom((lastName, context) =>
            {
                var result = LastName.Validate(lastName, context.PropertyName);
                if (result.IsFailure)
                    foreach (var error in result.Error.Errors)
                        context.AddFailure(error);
            });
        }

        public static IRuleBuilderInitial<T, string> GenderMustBeValid<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Custom((gender, context) =>
            {
                var result = Gender.ValidateAndConvert(gender, context.PropertyName);
                if (result.IsFailure)
                    context.AddFailure(result.Error);
            });
        }

        public static IRuleBuilderInitial<T, string> RoleMustBeValid<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Custom((role, context) =>
            {
                var result = Role.ValidateAndConvert(role, context.PropertyName);
                if (result.IsFailure)
                    context.AddFailure(result.Error);
            });
        }
    }
}