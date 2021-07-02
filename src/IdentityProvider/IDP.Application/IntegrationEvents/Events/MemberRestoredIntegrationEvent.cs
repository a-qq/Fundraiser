using SharedKernel.Infrastructure.Concretes.Models;

namespace IDP.Application.IntegrationEvents.Events
{
    public sealed class MemberRestoredIntegrationEvent : IntegrationEvent
    {
        public MemberRestoredIntegrationEvent(string memberId)
        {
            MemberId = memberId;
        }

        public string MemberId { get; }
    }
}