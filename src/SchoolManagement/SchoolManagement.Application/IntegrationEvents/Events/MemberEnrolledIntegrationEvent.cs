using SchoolManagement.Domain.SchoolAggregate.Members;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Infrastructure.Concretes.Models;

namespace SchoolManagement.Application.IntegrationEvents.Events
{
    internal sealed class MemberEnrolledIntegrationEvent : IntegrationEvent
    {
        public MemberEnrolledIntegrationEvent(
            MemberId memberId, string email, string role, SchoolId schoolId,
            string firstName, string lastName, string gender)
        {
            MemberId = memberId;
            Email = email;
            Role = role;
            SchoolId = schoolId;
            FirstName = firstName;
            LastName = lastName;
            Gender = gender;
        }

        public MemberId MemberId { get; }
        public string Email { get; }
        public string Role { get; }
        public SchoolId SchoolId { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Gender { get; }
    }
}