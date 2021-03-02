using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;
using SharedKernel.Domain.Errors;

namespace SchoolManagement.Domain.SchoolAggregate.Groups
{
    public class Sign : ValueObject
    {
        private Sign(string sign)
        {
            Value = sign;
        }

        private static int MaxLength => 4;
        public string Value { get; }

        public static Result<Sign, Error> Create(string sign)
        {
            var validation = Validate(sign);
            if (validation.IsFailure)
                return validation.ConvertFailure<Sign>();

            sign = sign.Trim();

            return new Sign(sign);
        }

        public static Result<bool, Error> Validate(string sign, string propertyName = nameof(Sign))
        {
            if (string.IsNullOrWhiteSpace(sign))
                return Result.Failure<bool, Error>(new Error($"{propertyName} is required!"));

            sign = sign.Trim();

            return Result.Combine(
                Result.FailureIf(sign.Length > 4, true,
                    new Error($"{propertyName} should consist of max {MaxLength} characters!")),
                Result.FailureIf(!sign.All(c => char.IsLetter(c)), true,
                    new Error($"{propertyName} should consist of only letters!")));
        }

        public static implicit operator string(Sign sign)
        {
            return sign.Value;
        }

        public override string ToString()
        {
            return Value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value.ToLower();
        }
    }
}