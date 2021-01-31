using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.RequestErrors;
using Fundraiser.SharedKernel.Utils;
using System;

namespace SchoolManagement.Data.Schools.ChangeGroupAssignment
{
    public sealed class ChangeGroupAssignmentCommand : ICommand<Result<bool, RequestError>>
    {
        public Guid StudentId { get; }
        public long GroupId { get; }
        public Guid SchoolId { get; }
        public Guid AuthId { get; }

        public ChangeGroupAssignmentCommand(Guid studentId, long groupId, Guid schoolId, Guid authId)
        {
            StudentId = studentId;
            GroupId = groupId;
            SchoolId = schoolId;
            AuthId = authId;
        }
    }
}
