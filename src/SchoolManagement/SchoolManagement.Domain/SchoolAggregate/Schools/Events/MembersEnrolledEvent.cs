using Ardalis.GuardClauses;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Domain.Common;
using SharedKernel.Domain.Extensions;
using System;
using System.Collections.Generic;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.Events
{
    public sealed class MembersEnrolledEvent : DomainEvent
    {
        public IEnumerable<Guid> MemberIds { get; }

        internal MembersEnrolledEvent(IEnumerable<MemberId> membersId)
        {
            MemberIds = Guard.Against.NullOrEmpty(membersId, nameof(membersId))
                .GuardEachAgainstDefault(nameof(membersId)).ConvertToGuid();
        }
    }
}
