using FundraiserManagement.Domain.MemberAggregate;

namespace FundraiserManagement.Application.Common.Models
{
    public sealed class MemberArchivisationData
    {
        public MemberId MemberId { get; }
        public string GroupRole { get; }

        public MemberArchivisationData(MemberId memberId, string groupRole)
        {
            MemberId = memberId;
            GroupRole = groupRole;
        }
    }
}
