using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using FundraiserManagement.Domain.FundraiserAggregate.Fundraisers;
using FundraiserManagement.Domain.FundraiserAggregate.Payments;
using FundraiserManagement.Domain.MemberAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using SharedKernel.Domain.Errors;

namespace FundraiserManagement.Domain.FundraiserAggregate.Participations
{
    public class Participation : Entity<ParticipationId>
    {
        private readonly List<Payment> _payments = new List<Payment>();

        public Fundraiser Fundraising { get; }
        public Member Participant { get; }

        public IReadOnlyList<Payment> Payments => _payments.AsReadOnly();

        private decimal GetEstimatedCurrentContribution()
             => _payments.Where(p => p.Status != Status.Failed).Sum(p => p.Amount);

        internal Participation(Fundraiser fundraising, Member participant)
        {
            Fundraising = Guard.Against.Null(fundraising, nameof(fundraising));
            Participant = Guard.Against.Null(participant, nameof(participant));
        }

        internal Result<PaymentId, Error> SavePayment(Amount amount, bool inCash, DateTime now)
        {
            if (Fundraising.State != State.Open)
                return new Error($"Fundraiser is not in an {State.Open.ToString().ToLower()} state!");

            if (Fundraising.Goal.IsShared)
            {
                var stake = Fundraising.GetStake();

                var currentContribution = GetEstimatedCurrentContribution();

                if (currentContribution + amount > stake)
                {
                    return new Error(
                        $"You cannot contribute more than {stake - currentContribution} to this fundraiser!");
                }
            }
            else
            {
                var balance = Fundraising.GetEstimatedBalance();

                if (balance >= Fundraising.Goal)
                {
                    return new Error(
                        "Final payments are being processed and goal might have been reached," +
                        " please try again later!");
                }

                if (balance + amount > Fundraising.Goal)
                {
                    return new Error(
                        $"You cannot contribute more than {Fundraising.Goal - balance} to this fundraiser!");
                }
            }

            var payment = new Payment(amount, inCash, Fundraising.Manager.Id, now);
            _payments.Add(payment);

            return payment.Id;
        }


        internal void CancelAllProcessingPayments(DateTime now)
        {
            foreach (var payment in Payments.Where(p => p.Status == Status.Processing))
                payment.Cancel(now);


        }
        protected Participation() { }
    }
}