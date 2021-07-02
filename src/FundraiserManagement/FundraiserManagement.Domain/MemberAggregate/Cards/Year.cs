using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace FundraiserManagement.Domain.MemberAggregate.Cards
{
    public class Year : ValueObject
    {
        public byte? Value { get; }

        public bool IsOutdated(DateTime now)
            => (Value < now.Year && !(Value == now.Year - 1 && now.DayOfYear < 5) || (Value > now.Year + 10));
              
        private Year(int year)
        {
            Value = (byte) year;
        }

        public static Result<Year> Create(int year, string propertyName = nameof(Year))
        {
            var validation = Validate(year, propertyName);

            if (validation.IsFailure)
                return validation.ConvertFailure<Year>();

            return Result.Success(new Year(year));
        }

        public static Result Validate(int year, string propertyName = nameof(Year))
        {
            if (year == 0)
                return Result.Failure($"{propertyName} is required!");

            if (year < 0 || year > 99)
                return Result.Failure($"{propertyName} is in wrong format!");

            return Result.Success();
        }

        public override string ToString()
            => Value.ToString();

        public static implicit operator int?(Year year)
            => year.Value;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
