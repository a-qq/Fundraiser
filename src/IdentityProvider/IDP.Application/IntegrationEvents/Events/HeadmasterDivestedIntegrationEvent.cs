using SharedKernel.Infrastructure.Concretes.Models;

namespace IDP.Application.IntegrationEvents.Events
{
    public sealed class HeadmasterDivestedIntegrationEvent : IntegrationEvent
    {
        public HeadmasterDivestedIntegrationEvent(
            string headmasterId, string removedRole, string assignedRole, bool isActive)
        {
            HeadmasterId = headmasterId;
            RemovedRole = removedRole;
            AssignedRole = assignedRole;
            IsActive = isActive;
        }

        public string HeadmasterId { get; }
        public string RemovedRole { get; }  
        public string AssignedRole { get; }
        public bool IsActive { get; }
    }
}