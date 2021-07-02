using System;

namespace FundraiserManagement.Domain.FundraiserAggregate.Fundraisers
{
    [Flags]
    public enum State
    {
        Preparation = 1,
        Open = 2,
        Stopped = 4,
        Closed = 8,
        GoalReached = 16,
        Suspended = 32,
        ResourcesPayedOut = 64,
        SuspendedAndOpened = Suspended | Open,
        SuspendedAndStopped = Suspended | Stopped
    }
}