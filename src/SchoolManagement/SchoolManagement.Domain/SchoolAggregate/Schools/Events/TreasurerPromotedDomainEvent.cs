using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Domain.Common;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.Events
{
    public sealed class TreasurerPromotedDomainEvent : DomainEvent
    {
        internal TreasurerPromotedDomainEvent(MemberId studentId, bool isActive)
        {
            StudentId = studentId;
            IsActive = isActive;
        }

        public MemberId StudentId { get; }
        public bool IsActive { get; }
    }
}