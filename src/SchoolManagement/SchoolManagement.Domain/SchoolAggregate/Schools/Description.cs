using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace SchoolManagement.Domain.SchoolAggregate.Schools
{
    public class Description : ValueObject
    {
        private Description(string value)
        {
            Value = value;
        }

        private static int MaxLength => 3000;
        public string Value { get; }

        public static Result<Description> Create(string description)
        {
            var validationResult = Validate(description);

            if (validationResult.IsFailure)
                return validationResult.ConvertFailure<Description>();

            return Result.Success(new Description(description));
        }

        public static Result Validate(string description, string propertyName = nameof(Description))
        {
            return Result.FailureIf(!string.IsNullOrEmpty(description) && description.Length > MaxLength,
                $"{propertyName} should contain max {MaxLength} characters!");
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static implicit operator string(Description description)
        {
            return description.Value;
        }
    }
}