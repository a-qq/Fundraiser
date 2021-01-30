using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.RequestErrors;
using Fundraiser.SharedKernel.Utils;
using System;

namespace SchoolManagement.Data.Schools.DeleteGroup
{
    public sealed class DeleteGroupCommand : ICommand<Result<bool, RequestError>>
    {
        public long GroupId { get; }
        public Guid SchoolId { get; }
        public Guid AuthId { get; }

        public DeleteGroupCommand(long groupId, Guid schoolId, Guid authId)
        {
            GroupId = groupId;
            SchoolId = schoolId;
            AuthId = authId;
        }
    }
}
