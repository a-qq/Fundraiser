using MediatR;
using System;

namespace SchoolManagement.Core.SchoolAggregate.Schools.Events
{
    public sealed class MemberArchivedEvent : INotification
    {
        public Guid MemberId { get; }

        public MemberArchivedEvent(Guid memberId)
        {
            MemberId = memberId == Guid.Empty ? throw new ArgumentNullException(nameof(memberId)) : memberId;
        }
    }
}
