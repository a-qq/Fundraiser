using MediatR;
using System;

namespace SchoolManagement.Core.SchoolAggregate.Schools.Events
{
    public sealed class FormTutorAssignedEvent : INotification
    {
        public Guid MemberId { get; }

        internal FormTutorAssignedEvent(Guid memberId)
        {
            MemberId = memberId == Guid.Empty ? throw new ArgumentNullException(nameof(memberId)) : memberId;
        }
    }
}
