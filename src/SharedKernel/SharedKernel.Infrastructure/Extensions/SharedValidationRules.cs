using FluentValidation;
using SharedKernel.Domain.ValueObjects;
using System;

namespace SharedKernel.Infrastructure.Extensions
{
    public static class SharedValidationRules
    {
        public static IRuleBuilderInitial<T, string> EmailMustBeValid<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Custom((email, context) =>
            {
                var result = Email.Validate(email, context.PropertyName);
                if (result.IsFailure)
                    foreach (var error in result.Error.Errors)
                        context.AddFailure(error);
            });
        }

        public static IRuleBuilderInitial<T, Guid> GuidIdMustBeValid<T>(this IRuleBuilder<T, Guid> ruleBuilder)
        {
            return ruleBuilder.Custom((id, context) =>
            {
                if (id == Guid.Empty)
                    context.AddFailure($"{context.PropertyName} is required and cannot be empty ('{context.PropertyValue}')");
            });
        }
    }
}