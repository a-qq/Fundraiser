using System.Collections.Generic;
using Ardalis.GuardClauses;
using SchoolManagement.Domain.SchoolAggregate.Groups;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Infrastructure.Concretes.Models;

namespace SchoolManagement.Application.IntegrationEvents.Events
{
    internal sealed class MembersEnrolledIntegrationEvent : IntegrationEvent
    {
        public MembersEnrolledIntegrationEvent(SchoolId schoolId, IReadOnlyCollection<MemberEnrollmentData> membersData)
        {
            Guard.Against.NullOrEmpty(membersData, nameof(membersData));

            SchoolId = schoolId;
            MembersData = membersData;
        }

        public SchoolId SchoolId { get; }
        public IReadOnlyCollection<MemberEnrollmentData> MembersData { get; }
    }

    internal sealed class MemberEnrollmentData
    {
        public MemberEnrollmentData(MemberId memberId, string email, string role,
            GroupId? groupId, string firstName, string lastName, string gender)
        {
            MemberId = memberId;
            Email = email;
            Role = role;
            GroupId = groupId;
            FirstName = firstName;
            LastName = lastName;
            Gender = gender;
        }

        public MemberId MemberId { get; }
        public string Email { get; }
        public string Role { get; }
        public GroupId? GroupId { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Gender { get; }
    }
}
