using CSharpFunctionalExtensions;
using IDP.Domain.UserAggregate.ValueObjects;

namespace IDP.Domain.Common.Errors
{
    internal static class IdentityProviderDomainError
    {
        public static class User
        {
            public static Result ExpiredSecurityCode()
                => Result.Failure("Security code expired!");
            public static Result RegistrationNotCompleted()
                => Result.Failure("Registration has not been completed registration!");

            public static Result RegistrationNotCompleted(Subject subject)
                => Result.Failure($"User {subject} has not completed registration!");

            public static Result AntiSpamIntervalNotPassed(int intervalInMinutes)
                => Result.Failure($"{intervalInMinutes} minutes have not expired since last request!");

        }
    }
}