using System;
using SharedKernel.Domain.Common;

namespace FundraiserManagement.Domain.Common.Models
{
    [StronglyTypedId]
    public partial struct SchoolId : ITypedId
    {
        public static implicit operator Guid(SchoolId schoolId)
        {
            return schoolId.Value;
        }
    }
}
