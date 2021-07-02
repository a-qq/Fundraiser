using System;
using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using FundraiserManagement.Domain.MemberAggregate;

namespace FundraiserManagement.Domain.FundraiserAggregate.Payments
{
    public class Payment : Entity<PaymentId>
    {
        public Amount Amount { get; }
        public DateTimeOffset AddedAt { get; }
        public DateTimeOffset? ProcessedAt { get; private set; }
        public Status Status { get; private set; }
        public MemberId ManagerId { get; }
        public bool InCash { get; }

        internal Payment(Amount amount, bool inCash, MemberId managerId, DateTime now)
        {
            Amount = Guard.Against.Null(amount, nameof(amount));
            Status = Status.Processing;
            AddedAt = now;
            ProcessedAt = null;
            InCash = inCash;
            ManagerId = managerId;
        }

        internal void Accept(DateTime now)
        {
            if (Status != Status.Processing)
                throw new InvalidOperationException(nameof(Payment)+" : " + nameof(Accept));

            Status = Status.Succeeded;
            ProcessedAt = now;
        }

        internal void Decline(DateTime now)
        {
            if (Status != Status.Processing)
                throw new InvalidOperationException(nameof(Payment) + " : " + nameof(Decline));

            Status = Status.Failed;
            ProcessedAt = now;
        }
        
        internal void Cancel(DateTime now)
        {
            if (Status != Status.Processing)
                throw new InvalidOperationException(nameof(Payment) + " : " + nameof(Cancel));

            Status = Status.Cancelled;
            ProcessedAt = now;
        }

        protected Payment() { }
    }
}
