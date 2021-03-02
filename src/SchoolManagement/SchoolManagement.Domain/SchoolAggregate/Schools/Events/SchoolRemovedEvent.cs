using System;
using Ardalis.GuardClauses;
using SharedKernel.Domain.Common;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.Events
{
    public sealed class SchoolRemovedEvent : DomainEvent
    {
        public SchoolRemovedEvent(SchoolId schoolId)
        {
            SchoolId = Guard.Against.Default(schoolId, nameof(schoolId));
        }

        public Guid SchoolId { get; }
    }
}