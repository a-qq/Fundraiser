using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.RequestErrors;
using Fundraiser.SharedKernel.Utils;
using System;

namespace SchoolManagement.Data.Schools.EditSchool.Admin
{
    public class EditSchoolCommand : ICommand<Result<bool, RequestError>>
    {
        public string Name { get; }
        public string Description { get; }
        public int? GroupMembersLimit { get; }
        public Guid AuthId { get; }
        public Guid SchoolId { get; }

        public EditSchoolCommand(string name, string description, int? groupMembersLimit, Guid authId, Guid schoolId)
        {
            Name = name;
            GroupMembersLimit = groupMembersLimit;
            Description = description;
            AuthId = authId;
            SchoolId = schoolId;
        }
    }
}
