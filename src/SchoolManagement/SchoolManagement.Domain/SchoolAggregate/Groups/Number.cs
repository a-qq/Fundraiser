using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace SchoolManagement.Domain.SchoolAggregate.Groups
{
    public class Number : ValueObject
    {
        private Number(byte number)
        {
            Value = number;
        }

        private static int MinValue => 1;
        private static int MaxValue => 4;
        public byte Value { get; }

        public static Result<Number> Create(int number)
        {
            var validation = Validate(number);
            if (validation.IsFailure)
                return validation.ConvertFailure<Number>();

            return new Number((byte) number);
        }

        public static Result Validate(int number, string propertyName = nameof(Number))
        {
            if (number < MinValue)
                return Result.Failure($"{propertyName} is required and must be at least {MinValue}!");

            if (number > MaxValue)
                return Result.Failure($"{propertyName} can not be greater than {MaxValue}!");

            return Result.Success();
        }

        public static implicit operator string(Number number)
        {
            return number.Value.ToString();
        }

        public static implicit operator int(Number number)
        {
            return number.Value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}