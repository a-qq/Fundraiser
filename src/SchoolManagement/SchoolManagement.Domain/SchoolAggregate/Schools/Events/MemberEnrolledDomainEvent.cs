using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Domain.Common;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.Events
{
    public sealed class MemberEnrolledDomainEvent : DomainEvent
    {
        internal MemberEnrolledDomainEvent(
            SchoolId schoolId, MemberId memberId)
        {
            SchoolId = schoolId;
            MemberId = memberId;
        }

        public SchoolId SchoolId { get; }
        public MemberId MemberId { get; }
    }
}