using MediatR;
using System;

namespace SchoolManagement.Core.SchoolAggregate.Schools.Events
{
    public sealed class MemberExpelledEvent : INotification
    {
        public Guid MemberId { get; }

        public MemberExpelledEvent(Guid memberId)
        {
            MemberId = memberId == Guid.Empty ? throw new ArgumentNullException(nameof(memberId)) : memberId;
        }
    }
}
