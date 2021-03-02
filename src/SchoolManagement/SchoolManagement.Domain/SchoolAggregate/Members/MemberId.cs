using System;
using SharedKernel.Domain.Common;

namespace SchoolManagement.Domain.SchoolAggregate.Members
{
    [StronglyTypedId(jsonConverter: StronglyTypedIdJsonConverter.SystemTextJson)]
    public partial struct MemberId : ITypedId
    {
        public static implicit operator Guid(MemberId memberId)
        {
            return memberId.Value;
        }
    }
}