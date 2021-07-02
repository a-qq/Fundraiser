using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;

namespace FundraiserManagement.Domain.MemberAggregate.Cards
{
    public class Cvc : ValueObject
    {
        public string Value { get; }

        private Cvc(string value)
        {
            Value = value.Trim();
        }

        public static Result<Cvc> Create(string cvc, string propertyName = nameof(Cvc))
        {
            var validation = Validate(cvc, propertyName);
            if (validation.IsFailure)
                return validation.ConvertFailure<Cvc>();

            return Result.Success(new Cvc(cvc));
        }

        public static Result Validate(string cvc, string propertyName = nameof(Cvc))
        {
            if (string.IsNullOrWhiteSpace(cvc))
                return Result.Failure($"{propertyName} is required!");

            cvc = cvc.Trim();

            if (cvc.Length != 3 || !cvc.All(char.IsDigit))
                return Result.Failure($"{propertyName} is invalid!");

            return Result.Success();
        }

        public override string ToString()
        {
            return Value;
        }

        public static implicit operator string(Cvc cvc)
        {
            return cvc.Value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
