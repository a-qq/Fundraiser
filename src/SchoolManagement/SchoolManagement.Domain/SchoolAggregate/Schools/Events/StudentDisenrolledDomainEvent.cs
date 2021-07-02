using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Domain.Common;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.Events
{
    public sealed class StudentDisenrolledDomainEvent : DomainEvent
    {
        internal StudentDisenrolledDomainEvent(MemberId studentId, string removedRole, bool isActive)
        {
            StudentId = studentId;
            RemovedRole = removedRole;
            IsActive = isActive;
        }

        public MemberId StudentId { get; }
        public string RemovedRole { get; }
        public bool IsActive { get; }
    }
}