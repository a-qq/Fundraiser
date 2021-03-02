using Ardalis.GuardClauses;
using SchoolManagement.Domain.SchoolAggregate.Groups;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Domain.Common;
using SharedKernel.Domain.Extensions;
using System;
using System.Collections.Generic;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.Events
{
    public sealed class StudentsAssignedEvent : DomainEvent
    {
        public Guid GroupId { get; }
        public IEnumerable<Guid> StudentIds { get; }

        public StudentsAssignedEvent(GroupId groupId, IEnumerable<MemberId> studentIds)
        {
            GroupId = Guard.Against.Default(groupId, nameof(groupId));

            StudentIds = Guard.Against.NullOrEmpty(studentIds, nameof(studentIds))
                .GuardEachAgainstDefault(nameof(studentIds)).ConvertToGuid();
        }
    }
}