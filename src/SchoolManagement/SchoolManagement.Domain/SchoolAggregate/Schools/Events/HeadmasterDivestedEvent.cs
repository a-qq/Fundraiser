using System;
using Ardalis.GuardClauses;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Domain.Common;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.Events
{
    public sealed class HeadmasterDivestedEvent : DomainEvent
    {
        internal HeadmasterDivestedEvent(MemberId headmasterId)
        {
            HeadmasterId = Guard.Against.Default(headmasterId, nameof(headmasterId));
        }

        public Guid HeadmasterId { get; }
    }
}