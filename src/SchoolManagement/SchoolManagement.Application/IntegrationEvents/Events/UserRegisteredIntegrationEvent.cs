using SharedKernel.Infrastructure.Concretes.Models;

namespace SchoolManagement.Application.IntegrationEvents.Events
{
    public sealed class UserRegisteredIntegrationEvent : IntegrationEvent
    {
        public string Subject { get; }

        public UserRegisteredIntegrationEvent(string subject)
        {
            Subject = subject;
        }
    }
}
