using System;
using SharedKernel.Domain.Common;

namespace FundraiserManagement.Domain.Common.Models
{
    [StronglyTypedId]
    public partial struct GroupId : ITypedId
    {
        public static implicit operator Guid(GroupId groupId)
        {
            return groupId.Value;
        }
    }
}
