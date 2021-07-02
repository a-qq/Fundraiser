using System;
using SharedKernel.Domain.Common;

namespace FundraiserManagement.Domain.FundraiserAggregate.Fundraisers
{
    [StronglyTypedId]
    public partial struct FundraiserId : ITypedId
    {
        public static implicit operator Guid(FundraiserId fundraiserId)
        {
            return fundraiserId.Value;
        }
    }
}
