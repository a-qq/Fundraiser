using SharedKernel.Infrastructure.Concretes.Models;
using System.Collections.Generic;
using IDP.Application.Common.Models;

namespace IDP.Application.IntegrationEvents.Events
{
    public sealed class MembersArchivedIntegrationEvent : IntegrationEvent
    {
        public IEnumerable<MemberArchivisationData> MembersData { get; }

        public MembersArchivedIntegrationEvent(IEnumerable<MemberArchivisationData> memberData)
        {
            MembersData = memberData;
        }
    }
}
