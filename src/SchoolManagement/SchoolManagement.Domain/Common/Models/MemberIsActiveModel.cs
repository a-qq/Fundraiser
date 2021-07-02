using SchoolManagement.Domain.SchoolAggregate.Members;

namespace SchoolManagement.Domain.Common.Models
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