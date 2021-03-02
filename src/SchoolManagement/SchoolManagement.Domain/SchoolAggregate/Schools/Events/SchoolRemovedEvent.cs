using Ardalis.GuardClauses;
using SharedKernel.Domain.Common;
using System;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.Events
{
    public sealed class SchoolRemovedEvent : DomainEvent
    {
        public Guid SchoolId { get; }

        public SchoolRemovedEvent(SchoolId schoolId)
        {
            SchoolId = Guard.Against.Default(schoolId, nameof(schoolId));
        }
    }
}
