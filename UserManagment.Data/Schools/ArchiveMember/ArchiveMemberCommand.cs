using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.RequestErrors;
using Fundraiser.SharedKernel.Utils;
using System;

namespace SchoolManagement.Data.Schools.ArchiveMember
{
    public sealed class ArchiveMemberCommand : ICommand<Result<bool, RequestError>> 
    {
        public Guid MemberId { get; }
        public Guid SchoolId { get; }
        public Guid AuthId { get; }

        public ArchiveMemberCommand(Guid memberId, Guid schoolId, Guid authId)
        {
            MemberId = memberId;
            SchoolId = schoolId;
            AuthId = authId;
        }
    }
}
