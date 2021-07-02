using System.Collections.Generic;
using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using SharedKernel.Domain.Errors;

namespace SchoolManagement.Domain.SchoolAggregate.Members
{
    public class LastName : ValueObject
    {
        private LastName(string value)
        {
            Value = value;
        }

        public static int MinLength => 2;
        public static int MaxLength => 200;
        public string Value { get; }


        public static Result<LastName,Error> Create(string lastName, string propertyName = nameof(LastName))
        {
            var validationResult = Validate(lastName, propertyName);
            if (validationResult.IsFailure)
                return validationResult.ConvertFailure<LastName>();

            lastName = lastName.Trim();

            var dashIndex = lastName.IndexOf('-');
            var offset = dashIndex > 0 
                ? lastName.Substring(1, dashIndex).ToLower() + char.ToUpper(lastName[dashIndex + 1]) + lastName.Substring(dashIndex+2).ToLower()
                : lastName.Substring(1).ToLower();

            lastName = char.ToUpper(lastName[0]) + offset;

            return new LastName(lastName);
        }

        public static Result<bool, Error> Validate(string lastName, string propertyName = nameof(LastName))
        {
            if (string.IsNullOrWhiteSpace(lastName))
                return Result.Failure<bool, Error>(new Error($"{propertyName} is required!"));

            lastName = lastName.Trim();
            return Result.Combine(
                Result.FailureIf(!Regex.IsMatch(lastName, @"^[\p{L}]+(-[\p{L}]+)?$"), true,
                    new Error($"{propertyName} should consist of only letters optionally divided by one '-'!")),
                Result.FailureIf(lastName.Length < MinLength, true,
                    new Error($"{propertyName} should consist of min {MinLength} characters!")),
                Result.FailureIf(lastName.Length > MaxLength, true,
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

        public static implicit operator string(LastName lastName)
        {
            return lastName.Value;
        }
    }
}