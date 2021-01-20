using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.ResultErrors;
using Fundraiser.SharedKernel.Utils;
using System;

namespace SchoolManagement.Data.Schools.CreateGroup
{
    public class CreateGroupCommand : ICommand<Result<GroupDTO, RequestError>>
    {
        public int Number { get; }
        public string Sign { get; }
        public Guid AuthId { get; }
        public Guid SchoolId { get; }

        public CreateGroupCommand(int number, string sign, Guid authId, Guid schoolId)
        {
            Number = number;
            Sign = sign;
            SchoolId = schoolId;
            AuthId = authId; 
        }
    }
}
