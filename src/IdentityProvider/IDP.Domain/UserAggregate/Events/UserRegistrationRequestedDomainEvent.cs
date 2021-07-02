using Ardalis.GuardClauses;
using IDP.Domain.UserAggregate.ValueObjects;
using SharedKernel.Domain.Common;

namespace IDP.Domain.UserAggregate.Events
{
    public sealed class UserRegistrationRequestedDomainEvent : DomainEvent    
    {
        public Subject Subject { get; }
        public UserRegistrationRequestedDomainEvent(Subject subject)
        {
            Subject = Guard.Against.Null(subject, nameof(subject));
        }
    }
}
