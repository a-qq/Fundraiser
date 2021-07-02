using System;
using SharedKernel.Domain.Common;

namespace FundraiserManagement.Domain.MemberAggregate
{
    [StronglyTypedId]
    public partial struct MemberId : ITypedId
    {
        public static implicit operator Guid(MemberId memberId)
        {
            return memberId.Value;
        }
    }
}