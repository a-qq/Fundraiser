using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.ResultErrors;
using Fundraiser.SharedKernel.Utils;
using System;

namespace SchoolManagement.Data.Schools.DivestFormTutor
{
    public sealed class DivestFormTutorCommand : ICommand<Result<bool, RequestError>>
    {
        public long GroupId { get; }
        public Guid SchoolId { get; }
        public Guid AuthId { get; }

        public DivestFormTutorCommand(long groupId, Guid schoolId, Guid authId)
        {
            GroupId = groupId;
            SchoolId = schoolId;
            AuthId = authId;
        }
    }
}
