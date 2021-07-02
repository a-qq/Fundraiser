using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Domain.Constants;
using SharedKernel.Infrastructure.Concretes.Models;

namespace SchoolManagement.Application.IntegrationEvents.Events
{
    internal sealed class HeadmasterPromotedIntegrationEvent : IntegrationEvent
    {
        public HeadmasterPromotedIntegrationEvent(MemberId teacherId, bool isActive)
        {
            TeacherId = teacherId;
            RemovedRole = SchoolRole.Teacher.ToString();
            AssignedRole = SchoolRole.Headmaster.ToString();
            IsActive = isActive;
        }

        public MemberId TeacherId { get; }
        public string RemovedRole { get; }
        public string AssignedRole { get; }
        public bool IsActive { get; }
    }
}