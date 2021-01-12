using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.ResultErrors;
using Fundraiser.SharedKernel.Utils;
using System;

namespace SchoolManagement.Data.Schools.EnrollMember
{
    public class EnrollMemberCommand : ICommand<Result<UserDTO, RequestError>>
    {
        public string FirstName { get; }
        public string LastName { get; }
        public string Email { get; }
        public string Role { get; }
        public string Gender { get; }
        public Guid SchoolId { get; }
        public Guid AuthId { get; }

        public EnrollMemberCommand(string firstName, string lastName, string email, string role, string gender, Guid authId, Guid schoolId)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Role = role;
            Gender = gender;
            SchoolId = schoolId == Guid.Empty ? throw new ArgumentNullException(nameof(schoolId)) : schoolId;
            AuthId = schoolId == Guid.Empty ? throw new ArgumentNullException(nameof(authId)) : authId;
        }
    }
}
