using MediatR;
using System;

namespace SchoolManagement.Core.SchoolAggregate.Schools.Events
{
    public sealed class TreasurerDivestedEvent : INotification
    {
        public Guid TreasurerId { get; }

        public TreasurerDivestedEvent(Guid treasurerId)
        {
            TreasurerId = treasurerId == Guid.Empty ? throw new ArgumentNullException(nameof(treasurerId)) : treasurerId;
        }
    }
}
