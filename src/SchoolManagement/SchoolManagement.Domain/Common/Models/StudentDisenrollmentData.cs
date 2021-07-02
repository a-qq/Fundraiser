using SchoolManagement.Domain.SchoolAggregate.Members;

namespace SchoolManagement.Domain.Common.Models
{
    public sealed class StudentDisenrollmentData
    {
        public MemberId MemberId { get; }
        public string GroupRole { get; }
        public bool IsActive { get; }

        public StudentDisenrollmentData(MemberId memberId, string groupRole, bool isActive)
        {
            MemberId = memberId;
            GroupRole = groupRole;
            IsActive = isActive;
        }
    }
}
