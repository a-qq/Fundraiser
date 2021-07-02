using FundraiserManagement.Domain.MemberAggregate;

namespace FundraiserManagement.Application.Common.Models
{
    public class MemberIsActiveModel
    {
        public MemberId MemberId { get; }
        public bool IsActive { get; }

        public MemberIsActiveModel(MemberId memberId, bool isActive)
        {
            MemberId = memberId;
            IsActive = isActive;
        }
    }
}