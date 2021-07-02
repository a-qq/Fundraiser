using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Domain.Common;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.Events
{
    public sealed class MemberArchivedDomainEvent : DomainEvent
    {
        internal MemberArchivedDomainEvent(
            MemberId memberId, string groupRole)
        {
            MemberId = memberId;
            GroupRole = groupRole;
        }

        public MemberId MemberId { get; }
        public string GroupRole { get; }
    }
}