using Ardalis.GuardClauses;
using SchoolManagement.Domain.Common.Models;
using SharedKernel.Infrastructure.Concretes.Models;
using System.Collections.Generic;

namespace SchoolManagement.Application.IntegrationEvents.Events
{
    internal sealed class MembersArchivedIntegrationEvent : IntegrationEvent
    {
        public IEnumerable<MemberArchivisationData> MembersData { get; }

        public MembersArchivedIntegrationEvent(IEnumerable<MemberArchivisationData> membersData)
        {
            MembersData = Guard.Against.NullOrEmpty(membersData, nameof(membersData));
        }
    }
}
