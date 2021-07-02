using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace FundraiserManagement.Domain.FundraiserAggregate.Fundraisers
{
    public class Goal : ValueObject
    {
        public decimal Value { get; }
        public bool IsShared { get; }

        public static decimal MaxValue = 50000000;
        private Goal(decimal value, bool isShared)
        {
            Value = value;
            IsShared = isShared;
        }

        public static Result<Goal> Create(decimal value, bool isShared, string propertyName = nameof(Goal))
        {
            var validation = Validate(value, propertyName);

            if (validation.IsFailure)
                return validation.ConvertFailure<Goal>();

            return Result.Success(new Goal(value, isShared));
        }

        public static Result Validate(decimal value, string propertyName = nameof(Goal))
        {
            if (value == 0)
                return Result.Failure($"{propertyName} is required!");

            if (value < 0)
                return Result.Failure($"{propertyName} is invalid!");

            if (value > MaxValue)
                return Result.Failure($"{propertyName} can't be greater than {MaxValue}!");

            return Result.Success();
        }

        public override string ToString()
            => $"{Value:C}";

        public static implicit operator decimal(Goal goal)
            => goal.Value;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
