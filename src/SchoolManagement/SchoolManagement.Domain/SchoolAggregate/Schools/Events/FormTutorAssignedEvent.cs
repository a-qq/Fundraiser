using Ardalis.GuardClauses;
using SchoolManagement.Domain.SchoolAggregate.Groups;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Domain.Common;
using System;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.Events
{
    public sealed class FormTutorAssignedEvent : DomainEvent
    {
        public Guid MemberId { get; }
        public Guid GroupId { get; }

        internal FormTutorAssignedEvent(MemberId memberId, GroupId groupId)
        {
            MemberId = Guard.Against.Default(memberId, nameof(memberId));
            GroupId = Guard.Against.Default(groupId, nameof(groupId));
        }
    }
}