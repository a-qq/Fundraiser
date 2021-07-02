using SharedKernel.Infrastructure.Concretes.Models;
using System.Collections.Generic;
using IDP.Application.Common.Models;

namespace IDP.Application.IntegrationEvents.Events
{
    public sealed class TreasurersDivestedIntegrationEvent : IntegrationEvent
    {
        public string RemovedRole { get; }
        public IEnumerable<MemberIsActiveModel> TreasurersData { get; }

        internal TreasurersDivestedIntegrationEvent(string removedRole, IEnumerable<MemberIsActiveModel> treasurersData)
        {
            RemovedRole = removedRole;
            TreasurersData = treasurersData;
        }
    }
}