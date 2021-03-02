using SharedKernel.Domain.Common;
using System;

namespace SchoolManagement.Domain.SchoolAggregate.Groups
{
    [StronglyTypedId(jsonConverter: StronglyTypedIdJsonConverter.SystemTextJson)]
    public partial struct GroupId : ITypedId
    { 
        public static implicit operator Guid(GroupId groupId)
            => groupId.Value;
    } 
}
