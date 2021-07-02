using SharedKernel.Infrastructure.Concretes.Models;

namespace IDP.Application.IntegrationEvents.Events
{
    public sealed class TreasurerPromotedIntegrationEvent : IntegrationEvent
    {
        public TreasurerPromotedIntegrationEvent(string studentId, string assignedRole, bool isActive)
        {
            StudentId = studentId;
            AssignedRole = assignedRole;
            IsActive = isActive;
        }

        public string StudentId { get; }
        public string AssignedRole { get; }
        public bool IsActive { get; }
    }
}