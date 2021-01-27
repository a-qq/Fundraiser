using MediatR;
using System;

namespace SchoolManagement.Core.SchoolAggregate.Schools.Events
{
    public sealed class MemberEnrolledEvent : INotification
    {
        public Guid MemberId { get; }

        internal MemberEnrolledEvent(Guid memberId)
        {
            MemberId = memberId == Guid.Empty ? throw new ArgumentNullException(nameof(memberId)) : memberId;
        }
    }
}
