using CSharpFunctionalExtensions;

namespace IDP.Core.UserAggregate.Entities
{
    public class Claim : Entity
    {
        public string Type { get; }
        public string Value { get; }
    }
}
