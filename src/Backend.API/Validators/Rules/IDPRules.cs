using FluentValidation;
using IDP.Domain.UserAggregate.ValueObjects;

namespace Backend.API.Validators.Rules
{
    public static class IDPRules
    {
        public static IRuleBuilderInitial<T, string> PasswordMustBeValid<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Custom((password, context) =>
            {
                var result = HashedPassword.Validate(password, context.PropertyName);
                if (result.IsFailure)
                    foreach (var error in result.Error.Errors)
                        context.AddFailure(error);
            });
        }
    }
}