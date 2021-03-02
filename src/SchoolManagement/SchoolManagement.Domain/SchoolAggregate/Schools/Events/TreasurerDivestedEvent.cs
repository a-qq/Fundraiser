using Ardalis.GuardClauses;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Domain.Common;
using System;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.Events
{
    public sealed class TreasurerDivestedEvent : DomainEvent
    {
        public Guid TreasurerId { get; }

        internal TreasurerDivestedEvent(MemberId treasurerId)
        {
            TreasurerId = Guard.Against.Default(treasurerId, nameof(treasurerId));
        }
    }
}