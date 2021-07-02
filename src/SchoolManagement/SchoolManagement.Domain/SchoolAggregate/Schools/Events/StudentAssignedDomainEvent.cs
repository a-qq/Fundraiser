using SchoolManagement.Domain.SchoolAggregate.Groups;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Domain.Common;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.Events
{
    public class StudentAssignedDomainEvent : DomainEvent
    {
        public MemberId StudentId { get; }
        public GroupId GroupId { get; }
        public bool IsActive { get; }

        internal StudentAssignedDomainEvent(MemberId studentId, GroupId groupId, bool isActive)
        {
            StudentId = studentId;
            GroupId = groupId;
            IsActive = isActive;
        }
    }
}