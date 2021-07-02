using Ardalis.GuardClauses;
using SchoolManagement.Domain.Common.Models;
using SharedKernel.Domain.Common;
using System.Collections.Generic;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.Events
{
    public sealed class MembersArchivedDomainEvent : DomainEvent
    {
        public IEnumerable<MemberArchivisationData> MembersData { get; }

        internal MembersArchivedDomainEvent(IEnumerable<MemberArchivisationData> membersData)
        {
            MembersData = Guard.Against.NullOrEmpty(membersData, nameof(membersData));
        }
    }
}
