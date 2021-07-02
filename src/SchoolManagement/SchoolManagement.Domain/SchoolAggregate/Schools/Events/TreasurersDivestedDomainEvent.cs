using Ardalis.GuardClauses;
using SchoolManagement.Domain.Common.Models;
using SharedKernel.Domain.Common;
using System.Collections.Generic;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.Events
{
    public sealed class TreasurersDivestedDomainEvent : DomainEvent
    {
        public IEnumerable<MemberIsActiveModel> TreasurerData { get; }

        internal TreasurersDivestedDomainEvent(IEnumerable<MemberIsActiveModel> treasurerData)
        {
            TreasurerData = Guard.Against.NullOrEmpty(treasurerData, nameof(treasurerData));
        }
    }
}