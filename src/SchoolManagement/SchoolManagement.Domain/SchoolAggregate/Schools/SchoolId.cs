using System;
using SharedKernel.Domain.Common;

namespace SchoolManagement.Domain.SchoolAggregate.Schools
{
    [StronglyTypedId(jsonConverter: StronglyTypedIdJsonConverter.SystemTextJson)]
    public partial struct SchoolId : ITypedId
    {
        public static implicit operator Guid(SchoolId schoolId)
        {
            return schoolId.Value;
        }
    }
}