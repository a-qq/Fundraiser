using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Infrastructure.Concretes.Models;

namespace SchoolManagement.Application.IntegrationEvents.Events
{
    internal sealed class MemberArchivedIntegrationEvent : IntegrationEvent
    {
        public MemberArchivedIntegrationEvent(MemberId memberId, string groupRole)
        {
            MemberId = memberId;
            GroupRole = groupRole;
        }

        public MemberId MemberId { get; }
        public string GroupRole { get; }
    }
}
