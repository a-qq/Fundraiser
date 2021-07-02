using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Domain.Common;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.Events
{
    public sealed class MemberRestoredDomainEvent : DomainEvent
    {
        internal MemberRestoredDomainEvent(MemberId memberId)
        {
            MemberId = memberId;
        }

        public MemberId MemberId { get; }
    }
}