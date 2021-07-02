using SchoolManagement.Domain.Common.Models;
using SharedKernel.Domain.Constants;
using SharedKernel.Infrastructure.Concretes.Models;
using System.Collections.Generic;

namespace SchoolManagement.Application.IntegrationEvents.Events
{
    public sealed class TreasurersDivestedIntegrationEvent : IntegrationEvent
    {
        public string RemovedRole { get; }
        public IEnumerable<MemberIsActiveModel> TreasurersData { get; }

        public TreasurersDivestedIntegrationEvent(IEnumerable<MemberIsActiveModel> treasurersData)
        {
            RemovedRole = GroupRoles.Treasurer;
            TreasurersData = treasurersData;
        }
    }
}