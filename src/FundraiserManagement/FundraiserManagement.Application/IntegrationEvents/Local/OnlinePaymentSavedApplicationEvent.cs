using FundraiserManagement.Domain.FundraiserAggregate.Payments;
using SharedKernel.Infrastructure.Concretes.Models;

namespace FundraiserManagement.Application.IntegrationEvents.Local
{
    public sealed class OnlinePaymentSavedApplicationEvent : IntegrationEvent
    {
        public PaymentId PaymentId { get; }

        public OnlinePaymentSavedApplicationEvent(PaymentId paymentId)
        {
            PaymentId = paymentId;
        }
    }
}