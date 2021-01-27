using CSharpFunctionalExtensions;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Fundraiser.SharedKernel.Utils
{
    public class Email : ValueObject
    {
        private static int MaxLength { get => 200; }
        public string Value { get; }
        private Email(string value)
        {
            Value = value;
        }

        public static Result<Email> Create(string email)
        {
            var validationResult = Validate(email);

            if (validationResult.IsFailure)
                return Result.Failure<Email>(string.Join(" ", validationResult.Error.Errors));

            email = email.Trim();

            return Result.Success(new Email(email));
        }
        public static Result<bool, Error> Validate(string email, string propertyName = nameof(Email))
        {
            if (string.IsNullOrWhiteSpace(email))
                return Result.Failure<bool, Error>(new Error($"{propertyName} is required!"));

            email = email.Trim();

            return Result.Combine( 
                Result.FailureIf(!Regex.IsMatch(email, @"^(.+)@(.+)$"), true, new Error($"{propertyName} is invalid!")),
                Result.FailureIf(email.Length > MaxLength, true, new Error($"{propertyName} should contain max {MaxLength} characters!")));
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString()
        {
            return this.Value;
        }

        public static implicit operator string(Email email)
        {
            return email.Value;
        }
    }
}
