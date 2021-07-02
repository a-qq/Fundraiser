using Ardalis.GuardClauses;
using IDP.Domain.UserAggregate.ValueObjects;
using SharedKernel.Domain.Common;

namespace IDP.Domain.UserAggregate.Events
{
    public sealed class PasswordResetRequestedDomainEvent : DomainEvent
    {
        internal PasswordResetRequestedDomainEvent(Subject subject)
        {
            Subject = Guard.Against.Null(subject, nameof(subject));
        }

        public Subject Subject { get; }
    }
}