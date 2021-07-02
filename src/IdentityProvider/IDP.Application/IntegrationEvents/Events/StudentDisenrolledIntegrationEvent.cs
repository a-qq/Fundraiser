using SharedKernel.Infrastructure.Concretes.Models;

namespace IDP.Application.IntegrationEvents.Events
{
    public sealed class StudentDisenrolledIntegrationEvent : IntegrationEvent
    {
        public string StudentId { get; }
        public string RemovedRole { get; }
        public bool IsActive { get; }

        public StudentDisenrolledIntegrationEvent(string studentId, string removedRole, bool isActive)
        {
            StudentId = studentId;
            RemovedRole = removedRole;
            IsActive = isActive;
        }
    }
}