using System;
using SharedKernel.Domain.Common;

namespace FundraiserManagement.Domain.FundraiserAggregate.Payments
{
    [StronglyTypedId]
    public partial struct PaymentId : ITypedId
    {
        public static implicit operator Guid(PaymentId paymentId)
        {
            return paymentId.Value;
        }
    }
}
