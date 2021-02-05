using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.RequestErrors;
using Fundraiser.SharedKernel.Utils;
using System;

namespace SchoolManagement.Data.Schools.Graduate
{
    public sealed class GraduateCommand : ICommand<Result<bool, RequestError>>
    {
        public Guid SchoolId { get; }
        public Guid AuthId { get; }

        public GraduateCommand(Guid schoolId, Guid authId)
        {
            SchoolId = schoolId;
            AuthId = authId;
        }
    }
}
