using SharedKernel.Infrastructure.Concretes.Models;

namespace IDP.Application.IntegrationEvents.Events
{
    public sealed class SchoolRemovedIntegrationEvent : IntegrationEvent
    {
        public SchoolRemovedIntegrationEvent(string schoolId)
        {
            SchoolId = schoolId;
        }

        public string SchoolId { get; }
    }
}
