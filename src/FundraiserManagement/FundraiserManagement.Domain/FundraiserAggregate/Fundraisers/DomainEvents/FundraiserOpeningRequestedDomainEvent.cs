using FundraiserManagement.Domain.Common.Models;
using FundraiserManagement.Domain.MemberAggregate;
using SharedKernel.Domain.Common;

namespace FundraiserManagement.Domain.FundraiserAggregate.Fundraisers.DomainEvents
{
    public sealed class FundraiserOpeningRequestedDomainEvent : DomainEvent
    {
        public FundraiserId FundraiserId { get; }
        public SchoolId SchoolId { get; }
        public MemberId? ExManagerId { get; }

        internal FundraiserOpeningRequestedDomainEvent(
            FundraiserId fundraiserId, SchoolId schoolId, MemberId? exManagerId = null)
        {
            FundraiserId = fundraiserId;
            SchoolId = schoolId;
            ExManagerId = exManagerId;
        }
    }
}