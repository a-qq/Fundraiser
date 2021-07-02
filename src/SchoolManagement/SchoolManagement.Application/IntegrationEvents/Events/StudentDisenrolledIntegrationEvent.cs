using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Infrastructure.Concretes.Models;

namespace SchoolManagement.Application.IntegrationEvents.Events
{
    internal sealed class StudentDisenrolledIntegrationEvent : IntegrationEvent
    {
        public MemberId StudentId { get; }
        public string RemovedRole { get; }
        public bool IsActive { get; }

        public StudentDisenrolledIntegrationEvent(MemberId studentId, string removedRole, bool isActive)
        {
            StudentId = studentId;
            RemovedRole = removedRole;
            IsActive = isActive;
        }
    }
}
