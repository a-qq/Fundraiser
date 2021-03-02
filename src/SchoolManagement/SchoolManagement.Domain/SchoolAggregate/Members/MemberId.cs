using SharedKernel.Domain.Common;
using System;

namespace SchoolManagement.Domain.SchoolAggregate.Members
{
    [StronglyTypedId(jsonConverter: StronglyTypedIdJsonConverter.SystemTextJson)]
    public partial struct MemberId : ITypedId
    {
        public static implicit operator Guid(MemberId memberId)
            => memberId.Value;
    }
}
