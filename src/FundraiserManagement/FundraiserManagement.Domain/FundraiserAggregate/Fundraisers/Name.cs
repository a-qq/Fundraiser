using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace FundraiserManagement.Domain.FundraiserAggregate.Fundraisers
{
    public class Name : ValueObject
    {
        private Name(string value)
        {
            Value = value;
        }

        public static int MaxLength => 500;
        public static int MinLength => 8;
        public string Value { get; }

        public static Result<Name> Create(string name, string propertyName = nameof(Name))
        {
            var validationResult = Validate(name, propertyName);

            if (validationResult.IsFailure)
                return validationResult.ConvertFailure<Name>();

            name = name.Trim();
            name = char.ToUpper(name[0]) + name.Substring(1).ToLower();

            return Result.Success(new Name(name));
        }

        public static Result Validate(string name, string propertyName = nameof(Name))
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result.Failure($"{propertyName} is required!");

            name = name.Trim();

            if (name.Length > MaxLength)
                return Result.Failure($"{propertyName} should contain max {MaxLength} characters!");

            if (name.Length < MinLength)
                return Result.Failure($"{propertyName} should contain min {MinLength} characters!");

            return Result.Success();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static implicit operator string(Name name)
        {
            return name.Value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}