using SharedKernel.Infrastructure.Concretes.Models;

namespace IDP.Application.IntegrationEvents.Events
{
    public sealed class PasswordResetRequestedIntegrationEvent : IntegrationEvent
    {
        public string Subject { get; }

        public PasswordResetRequestedIntegrationEvent(string subject)
        {
            Subject = subject;
        }
    }
}
