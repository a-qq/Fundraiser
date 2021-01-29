using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.RequestErrors;
using Fundraiser.SharedKernel.Utils;
using System;

namespace SchoolManagement.Data.Schools.DisenrollStudentFromGroup
{
    public sealed class DisenrollStudentFromGroupCommand : ICommand<Result<bool, RequestError>>
    {
        public long GroupId { get; }
        public Guid StudentId { get; }
        public Guid SchoolId { get; }
        public Guid AuthId { get; }

        public DisenrollStudentFromGroupCommand(long groupId, Guid studentId, Guid schoolId, Guid authId)
        {
            GroupId = groupId;
            StudentId = studentId;
            SchoolId = schoolId;
            AuthId = authId;
        }
    }
}
