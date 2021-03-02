using System;
using Ardalis.GuardClauses;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Domain.Common;

namespace SchoolManagement.Domain.SchoolAggregate.Schools.Events
{
    public sealed class TreasurerPromotedEvent : DomainEvent
    {
        internal TreasurerPromotedEvent(MemberId treasurerId)
        {
            TreasurerId = Guard.Against.Default(treasurerId, nameof(treasurerId));
        }

        public Guid TreasurerId { get; }
    }
}