using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.ResultErrors;
using Fundraiser.SharedKernel.Utils;
using SchoolManagement.Core.Interfaces;
using SchoolManagement.Core.SchoolAggregate.Groups;
using SchoolManagement.Core.SchoolAggregate.Schools;
using System;

namespace SchoolManagement.Core.SchoolAggregate.Users
{
    public class User : Entity<Guid>
    {
        public static readonly User Admin = new User(Guid.Parse("3f0b0a5f-7aa6-46f4-9247-3ef9a9df2407"));
        public FirstName FirstName { get; private set; }
        public LastName LastName { get; private set; }
        public Role Role { get; private set; }
        public Email Email { get; private set; }
        public Gender Gender { get; private set; }
        public bool IsActive { get; }
        public virtual School School { get; private set; }
        public virtual Group Group { get; private set; }


        protected User()
        {
        }
        private User(Guid id)
            : base(id)
        {
            Role = Role.Administrator;
        }

        private User(FirstName firstName, LastName lastName, School school, Email email, Role role, Gender gender)
            : base(Guid.NewGuid())
        {
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            School = school ?? throw new ArgumentNullException(nameof(school));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            Role = role ?? throw new ArgumentNullException(nameof(role));
            Gender = gender ?? throw new ArgumentNullException(nameof(gender));
        }


        public Result<School, RequestError> RegisterSchool(Name schoolName, FirstName headmasterFirstName, LastName headmasterLastName, 
            Email headmasterEmail, Gender headmasterGender)
        {
            var validationResult = CanRegisterSchool();

            if (validationResult.IsFailure)
                return validationResult.ConvertFailure<School>();

            School school = new School(schoolName);

            User Headmaster = new User(headmasterFirstName, headmasterLastName, school, headmasterEmail,
                Role.Headmaster, headmasterGender);

            school.Enroll(Headmaster);

            return Result.Success<School, RequestError>(school);
        }

        /// <summary>
        ///    Creates a new member and enroll him/her to school. Returns successfull or failure result depending on bussinsess rule validation.
        ///    Throws if calling User is an Administrator and <paramref name="school"/> isn't given.</summary>
        public Result<User, RequestError> CreateMemberAndEnrollToSchool(FirstName firstName, LastName lastName, Email email,
            Role role, Gender gender, School school = null) 
        {
            var validationResult = CanEnrollToSchool(school);

            if (validationResult.IsFailure)
                return validationResult.ConvertFailure<User>();

            if (school == null)
                school = this.School;

            User candidate = new User(firstName, lastName, school, email, role, gender);

            var enrollmentResult = school.Enroll(candidate);
            if (enrollmentResult.IsFailure)
                return enrollmentResult.ConvertFailure<User>();

            return Result.Success<User, RequestError>(candidate);
        }

        private Result<bool, RequestError> CanEnrollToSchool(School school)
        {
            if (Role < Role.Headmaster)
                return Result.Failure<bool, RequestError>(
                    SharedErrors.General.BusinessRuleViolation("Insufficient role for member enrollment!"));

            if (Role == Role.Administrator && school == null)
                throw new InvalidOperationException(nameof(CanEnrollToSchool));

            if (Role == Role.Headmaster && school != null && School != school)
                return Result.Failure<bool, RequestError>(
                    SharedErrors.General.BusinessRuleViolation("Headmaster can only enroll members to owned school!"));

            return Result.Success<bool, RequestError>(true);
        }

        private Result<bool, RequestError> CanRegisterSchool()
        {
            return Result.FailureIf(this.Role != Role.Administrator, true,
                SharedErrors.General.BusinessRuleViolation("Insufficient role for school registration!"));
        }
    }
}
