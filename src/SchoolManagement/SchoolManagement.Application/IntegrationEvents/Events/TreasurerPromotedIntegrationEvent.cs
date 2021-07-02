using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Domain.Constants;
using SharedKernel.Infrastructure.Concretes.Models;

namespace SchoolManagement.Application.IntegrationEvents.Events
{
    internal sealed class TreasurerPromotedIntegrationEvent : IntegrationEvent
    {
        public TreasurerPromotedIntegrationEvent(MemberId studentId, bool isActive)
        {
            StudentId = studentId;
            AssignedRole = GroupRoles.Treasurer;
            IsActive = isActive;
        }

        public MemberId StudentId { get; }
        public string AssignedRole { get; }
        public bool IsActive { get; }
    }
}