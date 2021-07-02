using SharedKernel.Infrastructure.Concretes.Models;

namespace IDP.Application.IntegrationEvents.Events
{
    public sealed class HeadmasterPromotedIntegrationEvent : IntegrationEvent
    {
        public HeadmasterPromotedIntegrationEvent(
            string teacherId, string removedRole, string assignedRole, bool isActive)
        {
            TeacherId = teacherId;
            RemovedRole = removedRole;
            AssignedRole = assignedRole;
            IsActive = isActive;
        }

        public string TeacherId { get; }
        public string RemovedRole { get; }
        public string AssignedRole { get; }
        public bool IsActive { get; }
    }
}