using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Infrastructure.Concretes.Models;

namespace SchoolManagement.Application.IntegrationEvents.Events
{
    internal sealed class SchoolRemovedIntegrationEvent : IntegrationEvent
    {
        public SchoolRemovedIntegrationEvent(SchoolId schoolId)
        {
            SchoolId = schoolId;
        }

        public SchoolId SchoolId { get; }
    }
}
