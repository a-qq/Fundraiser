using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.Utils;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SchoolManagement.Core.SchoolAggregate.Members
{
    public class LastName : ValueObject
    {
        private static int MinLength { get => 2; }
        private static int MaxLength { get => 200; }
        public string Value { get; }

        private LastName(string value)
        {
            Value = value;
        }


        public static Result<LastName> Create(string lastName)
        {
            var validationResult = Validate(lastName);
            if (validationResult.IsFailure)
                return Result.Failure<LastName>(string.Join(" ", validationResult.Error));

            lastName = lastName.Trim();
            lastName = char.ToUpper(lastName[0]) + lastName.Substring(1);

            return Result.Success(new LastName(lastName));
        }

        public static Result<bool, Error> Validate(string lastName, string properyName = nameof(LastName))
        {
            if (string.IsNullOrWhiteSpace(lastName))
                return Result.Failure<bool, Error>(new Error($"{properyName} is required!"));

            lastName = lastName.Trim();
            return Result.Combine(
                Result.FailureIf(!Regex.IsMatch(lastName, @"^[\p{L}]+-?[\p{L}]+$"), true, new Error($"{properyName} should consist of only letters optionally devided by one '-'!")),
                 Result.FailureIf(lastName.Length < MinLength, true, new Error($"{properyName} should consist of min {MinLength} characters!")),
                Result.FailureIf(lastName.Length > MaxLength, true, new Error($"{properyName} should contain max {MaxLength} characters!")));
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static implicit operator string(LastName lastName)
        {
            return lastName.Value;
        }
    }
}
