using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.RequestErrors;
using Fundraiser.SharedKernel.Utils;
using System;

namespace SchoolManagement.Data.Schools.DeleteSchool
{
    public sealed class DeleteSchoolCommand : ICommand<Result<bool, RequestError>>
    {
        public Guid SchoolId { get; }
        public Guid AuthId { get; }

        public DeleteSchoolCommand(Guid schoolId, Guid authId)
        {
            SchoolId = schoolId;
            AuthId = authId;
        }
    }
}
