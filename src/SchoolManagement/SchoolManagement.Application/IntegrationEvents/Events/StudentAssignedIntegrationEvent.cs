using SchoolManagement.Domain.SchoolAggregate.Groups;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Infrastructure.Concretes.Models;

namespace SchoolManagement.Application.IntegrationEvents.Events
{
    internal sealed class StudentAssignedIntegrationEvent : IntegrationEvent
    {
        public MemberId StudentId { get; }
        public GroupId GroupId { get; }
        public bool IsActive { get; }

        public StudentAssignedIntegrationEvent(MemberId studentId, GroupId groupId, bool isActive)
        {
            StudentId = studentId;
            GroupId = groupId;
            IsActive = isActive;
        }
    }
}
