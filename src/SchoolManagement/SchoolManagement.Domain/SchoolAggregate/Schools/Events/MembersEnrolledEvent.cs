using System;
using System.Collections.Generic;
using Ardalis.GuardClauses;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Domain.Common;
using SharedKernel.Domain.Extensions;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.Events
{
    public sealed class MembersEnrolledEvent : DomainEvent
    {
        internal MembersEnrolledEvent(IEnumerable<MemberId> membersId)
        {
            MemberIds = Guard.Against.NullOrEmpty(membersId, nameof(membersId))
                .GuardEachAgainstDefault(nameof(membersId)).ConvertToGuid();
        }

        public IEnumerable<Guid> MemberIds { get; }
    }
}