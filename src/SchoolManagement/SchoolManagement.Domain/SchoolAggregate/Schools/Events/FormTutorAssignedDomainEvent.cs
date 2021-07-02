using SchoolManagement.Domain.SchoolAggregate.Groups;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Domain.Common;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.Events
{
    public sealed class FormTutorAssignedDomainEvent : DomainEvent
    {
        internal FormTutorAssignedDomainEvent(
            MemberId teacherId, GroupId groupId, bool isActive)
        {
            TeacherId = teacherId;
            GroupId = groupId;
            IsActive = isActive;
        }

        public MemberId TeacherId { get; }
        public GroupId GroupId { get; }
        public bool IsActive { get; }
    }
}