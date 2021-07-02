using FundraiserManagement.Domain.FundraiserAggregate.Payments;
using SharedKernel.Domain.Common;

namespace FundraiserManagement.Domain.FundraiserAggregate.Fundraisers.DomainEvents
{
    public sealed class OnlinePaymentSavedDomainEvent : DomainEvent
    {
       public PaymentId PaymentId { get; }

        internal OnlinePaymentSavedDomainEvent(PaymentId paymentId)
        {
            PaymentId = paymentId;
        }
    }
}