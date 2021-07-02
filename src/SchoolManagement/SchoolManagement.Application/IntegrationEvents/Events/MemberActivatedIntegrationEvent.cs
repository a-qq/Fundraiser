using SchoolManagement.Domain.SchoolAggregate.Groups;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Domain.Constants;
using SharedKernel.Infrastructure.Concretes.Models;
using Gender = SharedKernel.Domain.Constants.Gender;

namespace SchoolManagement.Application.IntegrationEvents.Events
{
    public sealed class MemberActivatedIntegrationEvent : IntegrationEvent
    {
        public MemberId MemberId { get; }
        public SchoolId SchoolId { get; }
        public SchoolRole Role { get; }
        public Gender Gender { get; }
        public string Email { get; }
        public GroupId? GroupId { get; }
        public bool IsFormTutor { get; }
        public bool IsTreasurer { get; }

        public MemberActivatedIntegrationEvent(MemberId memberId, SchoolId schoolId, SchoolRole role,
            Gender gender, string email, GroupId? groupId, bool isFormTutor, bool isTreasurer)
        {
            MemberId = memberId;
            SchoolId = schoolId;
            Role = role;
            Gender = gender;
            Email = email;
            GroupId = groupId;
            IsFormTutor = isFormTutor;
            IsTreasurer = isTreasurer;
        }
    }
}
