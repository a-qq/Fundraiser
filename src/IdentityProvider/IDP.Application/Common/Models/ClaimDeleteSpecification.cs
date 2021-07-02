using CSharpFunctionalExtensions;

namespace IDP.Application.Common.Models
{
    public class ClaimDeleteSpecification
    {
        public string Type { get; }
        public Maybe<string> Value { get; }

        public ClaimDeleteSpecification(string type, string value = null)
        {
            Type = type;
            Value = value ?? Maybe<string>.None;
        }
    }
}
