using Ardalis.GuardClauses;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Domain.Common;
using System;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.Events
{
    public class HeadmasterPromotedEvent : DomainEvent
    {
        public Guid HeadmasterId { get; }

        internal HeadmasterPromotedEvent(MemberId headmasterId)
        {
            HeadmasterId = Guard.Against.Default(headmasterId, nameof(headmasterId));
        }
    }
}
