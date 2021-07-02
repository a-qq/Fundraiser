using FluentValidation;
using IDP.Domain.UserAggregate.ValueObjects;

namespace IDP.Application.Common.Validation
{
    public static class UserRules
    {
        public static IRuleBuilderInitial<T, string> SubjectMustBeValid<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Custom((property, context) =>
            {
                var result = Subject.Validate(property, context.PropertyName);
                if (result.IsFailure)
                    context.AddFailure(result.Error);
            });
        }

        public static IRuleBuilderInitial<T, int?> HoursToExpireMustBeValid<T>(this IRuleBuilder<T, int?> ruleBuilder)
        {
            return ruleBuilder.Custom((property, context) =>
            {
                if (property is null) return;
                var result = HoursToExpire.Validate(property.Value, context.PropertyName);
                if (result.IsFailure)
                    context.AddFailure(result.Error);
            });
        }

        public static IRuleBuilderInitial<T, string> PasswordMustBeValid<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Custom((property, context) =>
            {
                var result = HashedPassword.Validate(property, context.PropertyName);
                if (result.IsFailure)
                    context.AddFailure(result.Error);
            });
        }

        public static IRuleBuilderInitial<T, string> SecurityCodeMustBeValid<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Custom((property, context) =>
            {
                var result = SecurityCode.ValidateCode(property, context.PropertyName);
                if (result.IsFailure)
                    context.AddFailure(result.Error);
            });
        }
    }
}
