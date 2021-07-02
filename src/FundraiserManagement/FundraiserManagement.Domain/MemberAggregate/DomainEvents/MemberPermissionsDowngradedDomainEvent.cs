using FundraiserManagement.Domain.Common.Models;
using SharedKernel.Domain.Common;

namespace FundraiserManagement.Domain.MemberAggregate.DomainEvents
{
    public sealed class MemberPermissionsDowngradedDomainEvent : DomainEvent
    {
        public MemberId MemberId { get; }
        public SchoolId SchoolId { get; }
        public bool WasHeadmaster { get; }

        internal MemberPermissionsDowngradedDomainEvent(
            MemberId memberId, SchoolId schoolId, bool wasHeadmaster = false)
        {
            MemberId = memberId;
            SchoolId = schoolId;
            WasHeadmaster = wasHeadmaster;
        }
    }
}