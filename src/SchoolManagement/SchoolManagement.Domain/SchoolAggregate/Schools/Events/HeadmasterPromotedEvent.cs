using System;
using Ardalis.GuardClauses;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Domain.Common;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.Events
{
    public class HeadmasterPromotedEvent : DomainEvent
    {
        internal HeadmasterPromotedEvent(MemberId headmasterId)
        {
            HeadmasterId = Guard.Against.Default(headmasterId, nameof(headmasterId));
        }

        public Guid HeadmasterId { get; }
    }
}