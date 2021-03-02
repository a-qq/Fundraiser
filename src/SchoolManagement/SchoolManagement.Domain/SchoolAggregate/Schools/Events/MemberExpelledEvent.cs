using Ardalis.GuardClauses;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Domain.Common;
using System;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.Events
{
    public sealed class MemberExpelledEvent : DomainEvent
    {
        public Guid MemberId { get; }

        internal MemberExpelledEvent(MemberId memberId)
        {
            MemberId = Guard.Against.Default(memberId, nameof(memberId));
        }
    }
}
