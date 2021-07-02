using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace FundraiserManagement.Domain.FundraiserAggregate.Fundraisers
{
    public class Description : ValueObject
    {
        private Description(string value)
        {
            Value = value;
        }

        public static int MaxLength => 3000;
        public string Value { get; }

        public static Result<Description> Create(string description, string propertyName = nameof(Description))
        {
            var validationResult = Validate(description, propertyName);

            if (validationResult.IsFailure)
                return validationResult.ConvertFailure<Description>();

            if (string.IsNullOrWhiteSpace(description))
                description = null;

            description = description?.Trim();

            return Result.Success(new Description(description));
        }

        public static Result Validate(string description, string propertyName = nameof(Description))
        {
            return Result.FailureIf(!(description is null) && description.Trim().Length > MaxLength,
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

        public override string ToString()
        {
            return Value;
        }
    }
}