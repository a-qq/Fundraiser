using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.ResultErrors;
using Fundraiser.SharedKernel.Utils;
using System;

namespace SchoolManagement.Data.Schools.RegisterSchool
{
    public class RegisterSchoolCommand : ICommand<Result<SchoolCreatedDTO, RequestError>>
    {
        public string Name { get; }
        public string HeadmasterFirstName { get; }
        public string HeadmasterLastName { get; }
        public string HeadmasterEmail { get; }
        public string HeadmasterGender { get; }
        public Guid AuthId { get; }

        public RegisterSchoolCommand(string name, string firstName, string lastName, string email, string gender, Guid authId)
        {
            Name = name;
            HeadmasterFirstName = firstName;
            HeadmasterLastName = lastName;
            HeadmasterEmail = email;
            HeadmasterGender = gender;
            AuthId = authId == Guid.Empty ? throw new ArgumentNullException(nameof(authId)) : authId; 
        }
    }
}
