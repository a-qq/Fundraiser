using MediatR;
using System;

namespace SchoolManagement.Core.SchoolAggregate.Schools.Events
{
    public sealed class TreasurerPromotedEvent : INotification
    {
        public Guid TreasurerId { get; }

        public TreasurerPromotedEvent(Guid treasurerId)
        {
            TreasurerId = treasurerId == Guid.Empty ? throw new ArgumentNullException(nameof(treasurerId)) : treasurerId;
        }
    }
}
