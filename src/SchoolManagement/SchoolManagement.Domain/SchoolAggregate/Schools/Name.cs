using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace SchoolManagement.Domain.SchoolAggregate.Schools
{
    public class Name : ValueObject
    {
        private Name(string value)
        {
            Value = value;
        }

        private static int MaxLength => 500;
        public string Value { get; }

        public static Result<Name> Create(string name)
        {
            var validationResult = Validate(name);

            if (validationResult.IsFailure)
                return validationResult.ConvertFailure<Name>();

            name = name.Trim();

            return Result.Success(new Name(name));
        }

        public static Result Validate(string name, string propertyName = nameof(Name))
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result.Failure($"{propertyName} is required!");
            name = name.Trim();
            if (name.Length > MaxLength)
                return Result.Failure($"{propertyName} should contain max {MaxLength} characters!");

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