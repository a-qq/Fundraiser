using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.Utils;
using System.Collections.Generic;
using System.Linq;

namespace SchoolManagement.Core.SchoolAggregate.Members
{
    public class FirstName : ValueObject
    {
        private static int MinLength { get => 2; }
        private static int MaxLength { get => 200; }
        public string Value { get; }

        private FirstName(string value)
        {
            Value = value;
        }


        public static Result<FirstName> Create(string firstName)
        {
            var validationResult = Validate(firstName);
            if (validationResult.IsFailure)
                return Result.Failure<FirstName>(string.Join(" ", validationResult.Error));

            firstName = firstName.Trim();
            firstName = char.ToUpper(firstName[0]) + firstName.Substring(1);
            return Result.Success(new FirstName(firstName));
        }

        public static Result<bool, Error> Validate(string firstName, string propertyName = nameof(FirstName))
        {
            if (string.IsNullOrWhiteSpace(firstName))
                return Result.Failure<bool, Error>(new Error($"{propertyName} is required!"));

            firstName = firstName.Trim();
            return Result.Combine(
                Result.FailureIf(!firstName.All(char.IsLetter), true, new Error($"{propertyName} should consist of only letters!")),
                Result.FailureIf(firstName.Length < MinLength, true, new Error($"{propertyName} should consist of min {MinLength} characters!")),
                Result.FailureIf(firstName.Length > MaxLength, true, new Error($"{propertyName} should contain max {MaxLength} characters!")));
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString()
        {
            return this.Value;
        }

        public static implicit operator string(FirstName firstName)
        {
            return firstName.Value;
        }
    }
}
