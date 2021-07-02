using SharedKernel.Infrastructure.Concretes.Models;

namespace IDP.Application.IntegrationEvents.Events
{
    public sealed class MemberArchivedIntegrationEvent : IntegrationEvent
    {
        public MemberArchivedIntegrationEvent(string memberId, string groupRole)
        {
            MemberId = memberId;
            GroupRole = groupRole;
        }

        public string MemberId { get; }
        public string GroupRole { get; }
    }
}
