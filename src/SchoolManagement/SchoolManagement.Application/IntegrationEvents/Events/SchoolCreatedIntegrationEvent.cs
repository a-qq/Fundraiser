using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Infrastructure.Concretes.Models;

namespace SchoolManagement.Application.IntegrationEvents.Events
{
    public sealed class SchoolCreatedIntegrationEvent : IntegrationEvent
    {
        public SchoolId SchoolId { get; }

        public SchoolCreatedIntegrationEvent(SchoolId schoolId)
        {
            SchoolId = schoolId;
        }
    }
}