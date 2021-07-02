using SharedKernel.Domain.Common;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.Events
{
    public sealed class SchoolCreatedDomainEvent : DomainEvent
    {
        public SchoolId SchoolId { get; }

        public SchoolCreatedDomainEvent(SchoolId schoolId)
        {
            SchoolId = schoolId;
        }
    }
}