using SchoolManagement.Domain.SchoolAggregate.Members;

namespace SchoolManagement.Domain.Common.Models
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
