using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.RequestErrors;
using Fundraiser.SharedKernel.Utils;
using System;

namespace SchoolManagement.Data.Schools.MakeTeacherFormTutor
{
    public sealed class MakeTeacherFormTutorCommand : ICommand<Result<bool, RequestError>>
    {
        public Guid TeacherId { get; }
        public long GroupId { get; }
        public Guid SchoolId { get; }
        public Guid AuthId { get; }

        public MakeTeacherFormTutorCommand(Guid teacherId, long groupId, Guid schoolId, Guid authId)
        {
            TeacherId = teacherId;
            GroupId = groupId;
            SchoolId = schoolId;
            AuthId = authId;
        }
    }
}
