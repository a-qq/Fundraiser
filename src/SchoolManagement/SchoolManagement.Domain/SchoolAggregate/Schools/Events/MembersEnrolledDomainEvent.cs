using Ardalis.GuardClauses;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Domain.Common;
using System.Collections.Generic;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.Events
{
    public sealed class MembersEnrolledDomainEvent : DomainEvent
    {
        internal MembersEnrolledDomainEvent(SchoolId schoolId, IEnumerable<MemberId> membersId)
        {

            SchoolId = schoolId;
            MemberIds = Guard.Against.NullOrEmpty(membersId, nameof(membersId)); ;
        }

        public SchoolId SchoolId { get; }
        public IEnumerable<MemberId> MemberIds { get; }
    }
}