using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.RequestErrors;
using Fundraiser.SharedKernel.Utils;
using System;

namespace SchoolManagement.Data.Schools.ExpellMember
{
    public sealed class ExpellMemberCommand : ICommand<Result<bool, RequestError>>
    {
        public Guid MemberId { get; }
        public Guid SchoolId { get; }
        public Guid AuthId { get; }

        public ExpellMemberCommand(Guid memberId, Guid schoolId, Guid authId)
        {
            MemberId = memberId;
            SchoolId = schoolId;
            AuthId = authId;
        }
    }
}
