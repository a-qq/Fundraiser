using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;
using SharedKernel.Domain.Errors;

namespace SchoolManagement.Domain.SchoolAggregate.Members
{
    public class FirstName : ValueObject
    {
        private FirstName(string value)
        {
            Value = value;
        }

        public static int MinLength => 2;
        public static int MaxLength => 200;
        public string Value { get; }


        public static Result<FirstName, Error> Create(string firstName, string propertyName = nameof(FirstName))
        {
            var validationResult = Validate(firstName, propertyName);
            if (validationResult.IsFailure)
                return validationResult.ConvertFailure<FirstName>();

            firstName = firstName.Trim();
            firstName = char.ToUpper(firstName[0]) + firstName.Substring(1).ToLower();
            return new FirstName(firstName);
        }

        public static Result<bool, Error> Validate(string firstName, string propertyName = nameof(FirstName))
        {
            if (string.IsNullOrWhiteSpace(firstName))
                return Result.Failure<bool, Error>(new Error($"{propertyName} is required!"));

            firstName = firstName.Trim();
            return Result.Combine(
                Result.FailureIf(!firstName.All(char.IsLetter), true,
                    new Error($"{propertyName} should consist of only letters!")),
                Result.FailureIf(firstName.Length < MinLength, true,
                    new Error($"{propertyName} should consist of min {MinLength} characters!")),
                Result.FailureIf(firstName.Length > MaxLength, true,
                    new Error($"{propertyName} should contain max {MaxLength} characters!")));
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString()
        {
            return Value;
        }

        public static implicit operator string(FirstName firstName)
        {
            return firstName.Value;
        }
    }
}