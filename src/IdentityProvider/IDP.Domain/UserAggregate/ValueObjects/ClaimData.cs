using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using System.Collections.Generic;

namespace IDP.Domain.UserAggregate.ValueObjects
{
    public class ClaimData : ValueObject
    {
        public string Type { get; }
        public string Value { get; }

        public ClaimData(string type, string value)
        {
            Type = Guard.Against.NullOrWhiteSpace(type, nameof(type));
            Value = Guard.Against.NullOrWhiteSpace(value, nameof(value));
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Type;
            yield return Value;
        }
    }
}
