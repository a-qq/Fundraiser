using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace FundraiserManagement.Domain.FundraiserAggregate.Payments
{
    public class Amount : ValueObject
    {
        public decimal Value { get; }

        public static decimal MaxValue => 1000;

        private Amount(decimal value)
        {
            Value = value;
        }

        public static Result<Amount> Create(decimal amount, string propertyName = nameof(Amount))
        {
            var validation = Validate(amount, propertyName);
            if (validation.IsFailure)
                return validation.ConvertFailure<Amount>();

            return Result.Success(new Amount(amount));
        }

        public static Result Validate(decimal amount, string propertyName = nameof(Amount))
        {
            if (amount == 0)
                return Result.Failure($"{propertyName} is required!");

            if (amount < 0)
                return Result.Failure($"{propertyName} is invalid!");

            if (amount > MaxValue)
                return Result.Failure($"{propertyName} cannot be greater than {MaxValue}!");

            return Result.Success();
        }

        public override string ToString()
            => Value.ToString();

        public static implicit operator decimal(Amount amount)
            => amount.Value;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}