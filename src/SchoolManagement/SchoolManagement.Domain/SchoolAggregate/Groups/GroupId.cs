using System;
using SharedKernel.Domain.Common;

namespace SchoolManagement.Domain.SchoolAggregate.Groups
{
    [StronglyTypedId(jsonConverter: StronglyTypedIdJsonConverter.SystemTextJson)]
    public partial struct GroupId : ITypedId
    {
        public static implicit operator Guid(GroupId groupId)
        {
            return groupId.Value;
        }
    }
}