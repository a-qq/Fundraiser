using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Domain.Common;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.Events
{
    public sealed class MemberExpelledDomainEvent : DomainEvent
    {
        internal MemberExpelledDomainEvent(MemberId memberId, bool isActive)
        {
            MemberId = memberId;
            IsActive = isActive;
        }

        public MemberId MemberId { get; }
        public bool IsActive { get; }
    }
}