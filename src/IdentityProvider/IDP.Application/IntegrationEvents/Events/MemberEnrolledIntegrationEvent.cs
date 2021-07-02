using SharedKernel.Infrastructure.Concretes.Models;

namespace IDP.Application.IntegrationEvents.Events
{
    public sealed class MemberEnrolledIntegrationEvent : IntegrationEvent
    {
        public MemberEnrolledIntegrationEvent(
            string memberId, string email, string role, string schoolId,
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

        public string MemberId { get; }
        public string Email { get; }
        public string Role { get; }
        public string SchoolId { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Gender { get; }
    }
}