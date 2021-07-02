using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Domain.Constants;
using SharedKernel.Infrastructure.Concretes.Models;

namespace SchoolManagement.Application.IntegrationEvents.Events
{
    internal sealed class TreasurerDivestedIntegrationEvent : IntegrationEvent
    {
        public TreasurerDivestedIntegrationEvent(MemberId treasurerId, bool isActive)
        {
            TreasurerId = treasurerId;
            IsActive = isActive;
            RemovedRole = GroupRoles.Treasurer;
        }

        public MemberId TreasurerId { get; }
        public string RemovedRole { get; }
        public bool IsActive { get; }
    }
}