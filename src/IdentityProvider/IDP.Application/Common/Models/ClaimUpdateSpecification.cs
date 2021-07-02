using CSharpFunctionalExtensions;

namespace IDP.Application.Common.Models
{
    public class ClaimUpdateSpecification
    {
        public ClaimUpdateSpecification(string type, string newValue, string oldValue = null)
        {
            Type = type;
            OldValue = oldValue ?? Maybe<string>.None;
            NewValue = newValue;
        }

        public string Type { get; }
        public Maybe<string> OldValue { get; }
        public string NewValue { get; }
    }
}
