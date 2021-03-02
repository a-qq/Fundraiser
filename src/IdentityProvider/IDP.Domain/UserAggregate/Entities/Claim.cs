using CSharpFunctionalExtensions;

namespace IDP.Domain.UserAggregate.Entities
{
    public class Claim : Entity
    {
        public string Type { get; }
        public string Value { get; }
    }
}
