using SharedKernel.Domain.Common;
using System;

namespace SchoolManagement.Domain.SchoolAggregate.Schools
{
    [StronglyTypedId(jsonConverter: StronglyTypedIdJsonConverter.SystemTextJson)]
    public partial struct SchoolId : ITypedId
    {
        public static implicit operator Guid(SchoolId schoolId)
            => schoolId.Value;
    }
}
