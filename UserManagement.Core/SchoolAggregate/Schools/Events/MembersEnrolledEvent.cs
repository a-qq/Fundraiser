using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SchoolManagement.Core.SchoolAggregate.Schools.Events
{
    public sealed class MembersEnrolledEvent : INotification
    {
        public IEnumerable<Guid> MemberIds { get; }

        public MembersEnrolledEvent(IEnumerable<Guid> membersId)
        {
            if (membersId == null || !membersId.Any() || membersId.Any(c => c == Guid.Empty))
                throw new ArgumentException(nameof(membersId));

            MemberIds = membersId;
        }
    }
}
