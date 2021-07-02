using CSharpFunctionalExtensions;
using SharedKernel.Infrastructure.Concretes.Models;
using System.Collections.Generic;

namespace IDP.Application.IntegrationEvents.Events
{
    public sealed class MembersEnrolledIntegrationEvent : IntegrationEvent
    {
        public MembersEnrolledIntegrationEvent(
            string schoolId, IEnumerable<MemberData> membersData)
        {
            SchoolId = schoolId;
            MembersData = membersData;
        }

        public string SchoolId { get; }
        public IEnumerable<MemberData> MembersData { get; }
    }

    public sealed class MemberData
    {
        public MemberData(
            string memberId, string email, string role, string groupId, 
            string firstName, string lastName, string gender)
        {
            MemberId = memberId;
            Email = email;
            Role = role;
            FirstName = firstName;
            LastName = lastName;
            Gender = gender;
            GroupId = groupId ?? Maybe<string>.None;
        }

        public string MemberId { get; }
        public string Email { get; }
        public string Role { get; }
        public Maybe<string> GroupId { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Gender { get; }
    }
}
