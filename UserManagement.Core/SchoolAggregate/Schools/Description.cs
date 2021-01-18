using CSharpFunctionalExtensions;
using System.Collections.Generic;

namespace SchoolManagement.Core.SchoolAggregate.Schools
{
    public class Description : ValueObject
    {
        private static int MaxLength { get => 3000; }
        public string Value { get; }
        private Description(string value)
        {
            Value = value;
        }

        public static Result<Description> Create(string description)
        {
            Result validationResult = Validate(description);

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
