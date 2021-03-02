using Ardalis.GuardClauses;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Domain.Common;
using System;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.Events
{
    public sealed class HeadmasterDivestedEvent : DomainEvent
    {
        public Guid HeadmasterId { get; }

        internal HeadmasterDivestedEvent(MemberId headmasterId)
        {
            HeadmasterId = Guard.Against.Default(headmasterId, nameof(headmasterId));
        }
    }
}
