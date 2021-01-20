using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.ResultErrors;
using Fundraiser.SharedKernel.Utils;
using System;

namespace SchoolManagement.Data.Schools.EditSchool.Headmaster
{
    public class EditSchoolInfoCommand : ICommand<Result<bool, RequestError>>
    {
        public string Description { get; }
        public int? GroupMembersLimit { get; }
        public Guid AuthId { get; }
        public Guid SchoolId { get; }

        public EditSchoolInfoCommand(string description, int? groupMembersLimit, Guid authId, Guid schooldId)
        {
            Description = description;
            GroupMembersLimit = groupMembersLimit;
            AuthId = authId;
            SchoolId = schooldId;
        }
    }
}
