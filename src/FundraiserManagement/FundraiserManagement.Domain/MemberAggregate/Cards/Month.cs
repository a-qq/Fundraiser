using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace FundraiserManagement.Domain.MemberAggregate.Cards
{
    public class Month : ValueObject
    {
        public byte? Value { get; }

        private Month(int value)
        {
            Value = (byte)value;
        }

        public static Result<Month> Create(int month, string propertyName = nameof(Month))
        {
            var validation = Validate(month, propertyName);

            if (validation.IsFailure)
                return validation.ConvertFailure<Month>();

            return Result.Success(new Month(month));
        }

        public static Result Validate(int month, string propertyName = nameof(Month))
        {
            if (month == 0)
                return Result.Failure($"{propertyName} is required!");

            if (month > 12 || month < 1)
                return Result.Failure($"{propertyName} is invalid!");

            return Result.Success();
        }

        public static implicit operator int?(Month month)
            => month.Value;

        public override string ToString()
            => Value.ToString();

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
