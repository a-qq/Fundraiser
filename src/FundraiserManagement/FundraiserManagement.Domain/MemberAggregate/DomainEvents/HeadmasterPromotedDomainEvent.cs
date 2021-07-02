using FundraiserManagement.Domain.Common.Models;
using SharedKernel.Domain.Common;

namespace FundraiserManagement.Domain.MemberAggregate.DomainEvents
{
    public sealed class HeadmasterPromotedDomainEvent : DomainEvent
    {
        public MemberId MemberId { get; }
        public SchoolId SchoolId { get; }

        internal HeadmasterPromotedDomainEvent(MemberId memberId, SchoolId schoolId)
        {
            MemberId = memberId;
            SchoolId = schoolId;
        }
    }
}
