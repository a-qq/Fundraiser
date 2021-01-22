using Fundraiser.SharedKernel.Utils;
using MediatR;
using SchoolManagement.Core.SchoolAggregate.Members;
using System;

namespace SchoolManagement.Core.SchoolAggregate.Schools.Events
{
    public sealed class MemberEnrolledEvent : INotification
    {
        public Guid MemberId { get; }
        public FirstName FirstName { get; }
        public LastName LastName { get; }
        public Email Email { get; }
        public Role Role { get; }
        public Gender Gender { get; }
        public Guid SchoolId { get; }

        internal MemberEnrolledEvent(Guid memberId, FirstName firstName, LastName lastName, Email email, Role role, Gender gender, Guid schoolId)
        {
            MemberId = memberId == Guid.Empty ? throw new ArgumentNullException(nameof(memberId)) : memberId;
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            Role = role ?? throw new ArgumentNullException(nameof(role));
            Gender = gender ?? throw new ArgumentNullException(nameof(gender));
            SchoolId = schoolId == Guid.Empty ? throw new ArgumentNullException(nameof(schoolId)) : schoolId;
        }
    }
}
