using Ardalis.GuardClauses;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Domain.Common;
using System;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.Events
{
    public sealed class MemberArchivedEvent : DomainEvent
    {
        public Guid MemberId { get; }

        internal MemberArchivedEvent(MemberId memberId)
        {
            MemberId = Guard.Against.Default(memberId, nameof(memberId));
        }
    }
}
