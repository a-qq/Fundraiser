using System;
using System.Collections.Generic;
using Ardalis.GuardClauses;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Domain.Common;
using SharedKernel.Domain.Extensions;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.Events
{
    public sealed class GraduationCompletedEvent : DomainEvent
    {
        internal GraduationCompletedEvent(
            IEnumerable<MemberId> archivedMemberIds,
            IEnumerable<MemberId> divestedFormTutorIds,
            IEnumerable<MemberId> divestedTreasurerIds)
        {
            ArchivedMemberIds = Guard.Against.Null(archivedMemberIds, nameof(archivedMemberIds))
                .GuardEachAgainstDefault(nameof(archivedMemberIds)).ConvertToGuid();

            DivestedFormTutorIds = Guard.Against.Null(divestedFormTutorIds, nameof(divestedFormTutorIds))
                .GuardEachAgainstDefault(nameof(divestedFormTutorIds)).ConvertToGuid();

            DivestedTreasurerIds = Guard.Against.Null(divestedTreasurerIds, nameof(divestedTreasurerIds))
                .GuardEachAgainstDefault(nameof(divestedTreasurerIds)).ConvertToGuid();
        }

        public IEnumerable<Guid> ArchivedMemberIds { get; }
        public IEnumerable<Guid> DivestedFormTutorIds { get; }
        public IEnumerable<Guid> DivestedTreasurerIds { get; }
    }
}