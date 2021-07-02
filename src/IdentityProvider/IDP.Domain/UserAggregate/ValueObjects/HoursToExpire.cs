using CSharpFunctionalExtensions;
using System.Collections.Generic;

namespace IDP.Domain.UserAggregate.ValueObjects
{
    public class HoursToExpire : ValueObject
    {
        public static readonly HoursToExpire Infinite = new HoursToExpire(null);
        private static int MaxValue => 744;
        private HoursToExpire(int? value)
        {
            Value = value;
        }

        public int? Value { get; }

        public bool IsInfinite
            => Value is null;

        public static Result<HoursToExpire> Create(int value)
        {
            var result = Validate(value);

            if (result.IsFailure)
                return result.ConvertFailure<HoursToExpire>();

            return new HoursToExpire(value);
        }
    

        public static Result Validate(
            int hours, string propertyName = nameof(HoursToExpire))
        {
            if (hours < 1)
                return Result.Failure($"{propertyName} must be greater than 0!");

            if (hours > MaxValue)
                return Result.Failure($"{propertyName} cannot be greater than {MaxValue}!");

            return Result.Success();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }


        public static implicit operator int?(HoursToExpire hours)
            => hours.Value;
    }
}