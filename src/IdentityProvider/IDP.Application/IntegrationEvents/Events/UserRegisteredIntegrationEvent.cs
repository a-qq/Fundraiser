using Ardalis.GuardClauses;
using SharedKernel.Infrastructure.Concretes.Models;

namespace IDP.Application.IntegrationEvents.Events
{
    public sealed class UserRegisteredIntegrationEvent : IntegrationEvent
    {
        public string Subject { get; }

        public UserRegisteredIntegrationEvent(string subject)
        {
            Subject = Guard.Against.NullOrWhiteSpace(subject, nameof(subject));
        }
    }
}
