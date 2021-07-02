using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Domain.Common;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.Events
{
    public class HeadmasterPromotedDomainEvent : DomainEvent
    {
        internal HeadmasterPromotedDomainEvent(MemberId teacherId, bool isActive)
        {
            TeacherId = teacherId;
            IsActive = isActive;
        }

        public MemberId TeacherId { get; }
        public bool IsActive { get; }
    }
}