using System;
using Ardalis.GuardClauses;
using SchoolManagement.Domain.SchoolAggregate.Groups;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Domain.Common;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.Events
{
    public sealed class FormTutorAssignedEvent : DomainEvent
    {
        internal FormTutorAssignedEvent(MemberId memberId, GroupId groupId)
        {
            MemberId = Guard.Against.Default(memberId, nameof(memberId));
            GroupId = Guard.Against.Default(groupId, nameof(groupId));
        }

        public Guid MemberId { get; }
        public Guid GroupId { get; }
    }
}