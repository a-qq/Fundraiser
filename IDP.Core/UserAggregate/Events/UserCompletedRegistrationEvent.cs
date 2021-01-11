using MediatR;
using System;

namespace IDP.Core.UserAggregate.Events
{
    public sealed class UserCompletedRegistrationEvent : INotification
    {
        public Guid UserId { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Role { get; }
        public string Gender { get; }
        public Guid SchoolId { get; }

        public UserCompletedRegistrationEvent(string subject, string firstName, string lastName, string role, string gender, string schoolId)
        {
            UserId = Guid.Parse(subject);
            FirstName = firstName;
            LastName = lastName;
            Role = role;
            Gender = gender;
            SchoolId = Guid.Parse(schoolId);
        }
    }
}
