using System;
using Ardalis.GuardClauses;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Domain.Common;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.Events
{
    public sealed class MemberRestoredEvent : DomainEvent
    {
        internal MemberRestoredEvent(MemberId memberId)
        {
            MemberId = Guard.Against.Default(memberId, nameof(memberId));
        }

        public Guid MemberId { get; }
    }
}