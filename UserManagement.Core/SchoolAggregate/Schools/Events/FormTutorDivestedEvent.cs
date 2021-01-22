using MediatR;
using System;

namespace SchoolManagement.Core.SchoolAggregate.Schools.Events
{
    public sealed class FormTutorDivestedEvent : INotification
    {
        public Guid MemberId { get; }

        internal FormTutorDivestedEvent(Guid memberId)
        {
            MemberId = memberId == Guid.Empty ? throw new ArgumentNullException(nameof(memberId)) : memberId;
        }
    }
}
