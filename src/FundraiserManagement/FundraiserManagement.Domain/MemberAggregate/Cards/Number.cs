using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;

namespace FundraiserManagement.Domain.MemberAggregate.Cards
{
    public class Number : ValueObject
    {
        public string Value { get; }

        private Number(string value)
        {
            Value = value.Trim();
        }

        public static Result<Number> Create(string number, string propertyName = nameof(Number))
        {
            var validation = Validate(number, propertyName);

            if (validation.IsFailure)
                return validation.ConvertFailure<Number>();

            return Result.Success(new Number(number));
        }

        public static Result Validate(string number, string propertyName = nameof(Number))
        {
            if (string.IsNullOrWhiteSpace(number))
                return Result.Failure($"{propertyName} is required!");

            number = number.Trim();

            if (number.Length != 16 || !number.All(char.IsDigit) || !CheckLuhn(number))
                return Result.Failure($"{propertyName} is invalid!");

            if (!(IsVisa(number) || IsMastercard(number)))
                return Result.Failure($"Unsupported provider. Only VISA and Mastercard is accepted!");

            return Result.Success();
        }

        private static bool CheckLuhn(string number)
        {
            var sum = 0;
            var shouldDouble = false;

            for (var i = number.Length - 1; i >= 0; i--)
            {
                var digit = int.Parse(number[i].ToString());
                if (shouldDouble)
                {
                    if ((digit *= 2) > 9)
                        digit -= 9;
                }
                sum += digit;
                shouldDouble = !shouldDouble;
            }

            return (sum % 10) == 0;
        }

        private static bool IsVisa(string number)
            => Regex.IsMatch(number, @"^4[0-9]{12}(?:[0-9]{3})?$");

        private static bool IsMastercard(string number)
            => Regex.IsMatch(number, @"^5[1-5][0-9]{14}$|^2(?:2(?:2[1-9]|[3-9][0-9])|[3-6][0-9][0-9]|7(?:[01][0-9]|20))[0-9]{12}$");

        public override string ToString()
            => Value;

        public static implicit operator string(Number number)
            => number.Value;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}