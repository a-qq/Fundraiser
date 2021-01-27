using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.RequestErrors;
using Fundraiser.SharedKernel.Utils;
using System;
using System.Collections.Generic;

namespace SchoolManagement.Data.Schools.AddStudentsToGroup
{
    public class AddStudentsToGroupCommand : ICommand<Result<bool, RequestError>>
    {
        public IReadOnlyList<Guid> StudentIds { get; }
        public Guid AuthId { get; }
        public Guid SchoolId { get; }
        public long GroupId { get; }

        public AddStudentsToGroupCommand(IEnumerable<Guid> studentIds, Guid authId, Guid schoolId, long groupId)
        {
            StudentIds = new List<Guid>(studentIds);
            AuthId = authId;
            SchoolId = schoolId;
            GroupId = groupId;
        }
    }
}
