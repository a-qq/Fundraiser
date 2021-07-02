using IDP.Application.Common.Models;
using SharedKernel.Infrastructure.Concretes.Models;
using System.Collections.Generic;

namespace IDP.Application.IntegrationEvents.Events
{
    public sealed class StudentsAssignedIntegrationEvent : IntegrationEvent
    {
        public StudentsAssignedIntegrationEvent(
            string groupId, IEnumerable<MemberIsActiveModel> membersData)
        {
            GroupId = groupId;
            MembersData = membersData;
        }

        public string GroupId { get; }
        public IEnumerable<MemberIsActiveModel> MembersData { get; }
    }
}