using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;
using IDP.Domain.UserAggregate.ValueObjects;
using SharedKernel.Domain.ValueObjects;

namespace IDP.Application.Common.Models
{
    internal static class IdentityProviderApplicationError
    {
        public static class User
        {
            public static Result NotFound(Subject subject)
                => Result.Failure($"User with subject '{subject}' not found!");

            public static Result NotFound(IReadOnlyCollection<Subject> subjects)
                => Result.Failure($"Users with subjects {string.Join(", ", subjects.Select(x => "'" + x + "'"))}' not found!");

            public static Result NotFound(string subject)
                => Result.Failure($"User with subject '{subject}' not found!");

            public static Result NotFound(Email email)
                => Result.Failure($"User with email '{email}' not found!");

            public static Result LoginFailed
                => Result.Failure("Invalid email or password!");

            public static Result InvalidSecurityCode
                => Result.Failure("Security code invalid!");
        }
    }
}
