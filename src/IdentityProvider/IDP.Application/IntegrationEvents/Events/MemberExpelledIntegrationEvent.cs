using SharedKernel.Infrastructure.Concretes.Models;

namespace IDP.Application.IntegrationEvents.Events
{
    public sealed class MemberExpelledIntegrationEvent : IntegrationEvent
    {
        public MemberExpelledIntegrationEvent(string memberId, bool isActive)
        {
            MemberId = memberId;
            IsActive = isActive;
        }

        public string MemberId { get; }
        public bool IsActive { get; }
    }
}