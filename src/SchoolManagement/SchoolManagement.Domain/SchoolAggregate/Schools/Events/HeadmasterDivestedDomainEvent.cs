using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Domain.Common;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.Events
{
    public sealed class HeadmasterDivestedDomainEvent : DomainEvent
    {
        internal HeadmasterDivestedDomainEvent(
            MemberId headmasterId, bool isActive)
        {
            HeadmasterId = headmasterId;
            IsActive = isActive;
        }

        public MemberId HeadmasterId { get; }
        public bool IsActive { get; }
    }
}