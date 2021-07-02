namespace IDP.Application.Common.Models
{
    public sealed class MemberArchivisationData
    {
        public string MemberId { get; }
        public string GroupRole { get; }

        public MemberArchivisationData(string memberId, string groupRole)
        {
            MemberId = memberId;
            GroupRole = groupRole;
        }
    }
}
