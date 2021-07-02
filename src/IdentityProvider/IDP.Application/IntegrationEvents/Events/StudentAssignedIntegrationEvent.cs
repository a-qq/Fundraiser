using SharedKernel.Infrastructure.Concretes.Models;

namespace IDP.Application.IntegrationEvents.Events
{
    public sealed class StudentAssignedIntegrationEvent : IntegrationEvent
    {
        public string StudentId { get; }
        public string GroupId { get; }
        public bool IsActive { get; }

        public StudentAssignedIntegrationEvent(string studentId, string groupId, bool isActive)
        {
            StudentId = studentId;
            GroupId = groupId;
            IsActive = isActive;
        }
    }
}
