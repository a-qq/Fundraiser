using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Domain.Constants;
using SharedKernel.Infrastructure.Concretes.Models;

namespace SchoolManagement.Application.IntegrationEvents.Events
{
    internal sealed class HeadmasterDivestedIntegrationEvent : IntegrationEvent
    {
        public HeadmasterDivestedIntegrationEvent(MemberId headmasterId, bool isActive)
        {
            HeadmasterId = headmasterId;
            IsActive = isActive;
            RemovedRole = SchoolRole.Headmaster.ToString();
            AssignedRole = SchoolRole.Teacher.ToString();
        }

        public MemberId HeadmasterId { get; }
        public string RemovedRole { get; }  
        public string AssignedRole { get; }
        public bool IsActive { get; }
    }
}