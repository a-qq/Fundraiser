using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Domain.Common;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.Events
{
    public sealed class MemberActivatedDomainEvent : DomainEvent
    {
        public SchoolId SchoolId { get; }
        public MemberId MemberId { get; }

        public MemberActivatedDomainEvent(SchoolId schoolId, MemberId memberId)
        {
            SchoolId = schoolId;
            MemberId = memberId;
        }
    }
}