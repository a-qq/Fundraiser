using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.ResultErrors;
using Fundraiser.SharedKernel.Utils;
using System;

namespace SchoolManagement.Data.Schools.EditSchool.Admin
{
    public class EditSchoolCommand : ICommand<Result<bool, RequestError>>
    {
        public string Name { get; }
        public string Description { get; }
        public Guid AuthId { get; }
        public Guid SchoolId { get; }

        public EditSchoolCommand(string name, string description, Guid authId, Guid schoolId)
        {
            Name = name;
            Description = description;
            AuthId = authId;
            SchoolId = schoolId;
        }

        public EditSchoolCommand(string description, Guid authId, Guid schoolId)
        {
            Description = description;
            AuthId = authId;
            SchoolId = schoolId;
        }
    }
}
