using SharedKernel.Infrastructure.Concretes.Models;

namespace IDP.Application.IntegrationEvents.Events
{
    public sealed class TreasurerDivestedIntegrationEvent : IntegrationEvent
    {
        public TreasurerDivestedIntegrationEvent(
            string treasurerId, string removedRole, bool isActive)
        {
            TreasurerId = treasurerId;
            RemovedRole = removedRole;
            IsActive = isActive;
        }

        public string TreasurerId { get; }
        public string RemovedRole { get; }
        public bool IsActive { get; }
    }
}
