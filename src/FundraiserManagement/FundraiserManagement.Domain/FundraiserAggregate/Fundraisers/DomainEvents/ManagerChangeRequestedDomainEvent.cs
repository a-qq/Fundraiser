using FundraiserManagement.Domain.Common.Models;
using FundraiserManagement.Domain.MemberAggregate;
using SharedKernel.Domain.Common;

namespace FundraiserManagement.Domain.FundraiserAggregate.Fundraisers.DomainEvents
{
    public sealed class ManagerChangeRequestedDomainEvent : DomainEvent
    {
        public FundraiserId FundraiserId { get; }
        public SchoolId SchoolId { get; }
        public MemberId MemberId { get; }

        internal ManagerChangeRequestedDomainEvent(
            SchoolId schoolId, FundraiserId fundraiserId, MemberId memberId)
        {
            FundraiserId = fundraiserId;
            MemberId = memberId;
            SchoolId = schoolId;
        }
    }
}