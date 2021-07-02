using SharedKernel.Infrastructure.Concretes.Models;

namespace IDP.Application.IntegrationEvents.Events
{
    public sealed class UserRegistrationRequestedIntegrationEvent : IntegrationEvent
    {
        public string Subject { get; }

        public UserRegistrationRequestedIntegrationEvent(string subject)
        {
            Subject = subject;
        }
    }
}
