using Ardalis.GuardClauses;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Domain.Common;
using SharedKernel.Domain.Extensions;
using System;
using System.Collections.Generic;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.Events
{
    public sealed class GraduationCompletedEvent : DomainEvent
    {
        public IEnumerable<Guid> ArchivedMemberIds { get; }
        public IEnumerable<Guid> DivestedFormTutorIds { get; }
        public IEnumerable<Guid> DivestedTreasurerIds { get; }

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
    }
}