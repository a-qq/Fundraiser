using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;

namespace IDP.Domain.UserAggregate.Entities
{
    public class Claim : Entity
    {
        public string Type { get; }
        public string Value { get; private set; }

        protected Claim() { }

        public Claim(string type, string value)
        {
            Type = Guard.Against.NullOrWhiteSpace(type, nameof(type));
            Value = Guard.Against.NullOrWhiteSpace(value, nameof(value));
        }

        internal void Update(string newValue)
            => Value = Guard.Against.NullOrWhiteSpace(newValue, nameof(newValue));
    }
}