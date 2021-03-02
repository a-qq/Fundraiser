using Ardalis.GuardClauses;
using SharedKernel.Domain.Common;
using System;

namespace IDP.Domain.UserAggregate.Events
{
    public sealed class UserCompletedRegistrationEvent : DomainEvent
    {
        public Guid UserId { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Role { get; }
        public string Gender { get; }
        public Guid SchoolId { get; }
        public Guid? GroupId { get; }

        public UserCompletedRegistrationEvent(string subject, string firstName, string lastName, string role, string gender, string schoolId, string groupId)
        {
            UserId = Guid.Parse(subject);
            FirstName = Guard.Against.NullOrWhiteSpace(firstName, nameof(firstName));
            LastName = Guard.Against.NullOrWhiteSpace(lastName, nameof(lastName));
            Role = Guard.Against.NullOrWhiteSpace(role, nameof(role));
            Gender = Guard.Against.NullOrWhiteSpace(gender, nameof(gender));
            SchoolId = Guid.Parse(schoolId);
            if (string.IsNullOrWhiteSpace(groupId))
                GroupId = null;
            else GroupId = Guid.Parse(groupId);
        }
    }
}
