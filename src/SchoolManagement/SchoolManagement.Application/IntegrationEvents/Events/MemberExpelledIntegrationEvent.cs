using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Infrastructure.Concretes.Models;

namespace SchoolManagement.Application.IntegrationEvents.Events
{
    internal sealed class MemberExpelledIntegrationEvent : IntegrationEvent
    {
        public MemberExpelledIntegrationEvent(MemberId memberId, bool isActive)
        {
            MemberId = memberId;
            IsActive = isActive;
        }

        public MemberId MemberId { get; }
        public bool IsActive { get; }
    }
}