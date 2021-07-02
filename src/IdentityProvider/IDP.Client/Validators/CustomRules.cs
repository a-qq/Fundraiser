using FluentValidation;
using IDP.Domain.UserAggregate.ValueObjects;

namespace IDP.Client.Validators
{
    public static class CustomRules
    {
        public static IRuleBuilderInitial<T, string> PasswordMustBeValid<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Custom((password, context) =>
            {
                var result = HashedPassword.Validate(password, context.PropertyName);
                if (result.IsFailure)
                    context.AddFailure(result.Error);
            });
        }
    }
}