using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Domain.Common;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.Events
{
    public sealed class TreasurerDivestedDomainEvent : DomainEvent
    {
        internal TreasurerDivestedDomainEvent(MemberId treasurerId, bool isActive)
        {
            TreasurerId = treasurerId;
            IsActive = isActive;
        }

        public MemberId TreasurerId { get; }
        public bool IsActive { get; }
    }
}