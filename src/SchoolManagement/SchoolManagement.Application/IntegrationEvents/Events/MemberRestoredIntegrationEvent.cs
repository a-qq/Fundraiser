using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Infrastructure.Concretes.Models;

namespace SchoolManagement.Application.IntegrationEvents.Events
{
    public sealed class MemberRestoredIntegrationEvent : IntegrationEvent
    {
        public MemberRestoredIntegrationEvent(MemberId memberId)
        {
            MemberId = memberId;
        }

        public MemberId MemberId { get; }
    }
}
