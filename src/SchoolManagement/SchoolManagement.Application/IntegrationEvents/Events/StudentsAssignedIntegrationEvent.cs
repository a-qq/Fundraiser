using Ardalis.GuardClauses;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Domain.SchoolAggregate.Groups;
using SharedKernel.Infrastructure.Concretes.Models;
using System.Collections.Generic;
using SchoolManagement.Domain.Common.Models;

namespace SchoolManagement.Application.IntegrationEvents.Events
{
    internal sealed class StudentsAssignedIntegrationEvent : IntegrationEvent
    {
        public StudentsAssignedIntegrationEvent(GroupId groupId, IEnumerable<MemberIsActiveModel> membersData)
        {
            GroupId = groupId;
            MembersData = Guard.Against.NullOrEmpty(membersData, nameof(membersData)); ;
        }

        public GroupId GroupId { get; }
        public IEnumerable<MemberIsActiveModel> MembersData { get; }
    }
}