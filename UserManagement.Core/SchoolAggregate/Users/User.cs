using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.Utils;
using SchoolManagement.Core.SchoolAggregate.Groups;
using SchoolManagement.Core.SchoolAggregate.Schools;
using System;

namespace SchoolManagement.Core.SchoolAggregate.Users
{
    public class User : Entity<Guid>
    {
        public static readonly User Admin = new User(Guid.Parse("3f0b0a5f-7aa6-46f4-9247-3ef9a9df2407"));
        public FirstName FirstName { get; }
        public LastName LastName { get; }
        public Role Role { get; private set; }
        public Email Email { get; }
        public Gender Gender { get; }
        public bool IsActive { get; }
        public virtual School School { get; }
        public virtual Group Group { get; private set; }


        protected User()
        {
        }

        private User(Guid id)
            : base(id)
        {
            Role = Role.Administrator;
        }

        private User(FirstName firstName, LastName lastName, Email email, Role role, Gender gender, School school)
            : base(Guid.NewGuid())
        {
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            Role = role ?? throw new ArgumentNullException(nameof(role));
            Gender = gender ?? throw new ArgumentNullException(nameof(gender));
            School = school ?? throw new ArgumentNullException(nameof(school));
        }

        internal static Result<User> Create(FirstName firstName, LastName lastName, Email email, Role role, Gender gender, School school)
        {
            User candidate = new User(firstName, lastName, email, role, gender, school);

            Result enrollment = school.Enroll(candidate);

            if (enrollment.IsFailure)
                return enrollment.ConvertFailure<User>();

            return Result.Success(candidate);
        }
        public Result<School> RegisterSchool(Name schoolName, FirstName headmasterFirstName, LastName headmasterLastName, 
            Email headmasterEmail, Gender headmasterGender)
        {
            if(this.Role != Role.Administrator)
                Result.Failure<School>("Insufficient role for school registration!");

            School school = new School(schoolName, headmasterFirstName, headmasterLastName, headmasterEmail, headmasterGender);

            return Result.Success(school);
        }

        /// <summary>
        ///    Creates and enrolls a new member to <paramref name="school"/>. Returns successfull or failure result depending on bussinsess rule validation.
        ///    Throws if calling User is a Headmaster and isn't member of a <paramref name="school"/>.</summary>
        public Result<User> EnrollToSchool(FirstName firstName, LastName lastName, Email email,
            Role role, Gender gender, School school) 
        {
            if (this.Role < Role.Headmaster)
                return Result.Failure<User>("Insufficient role for member enrollment!");

            if (this.Role == Role.Headmaster)
            {
                if (this.School != school)
                    throw new InvalidOperationException(nameof(EnrollToSchool));

                if (this.Role == role) //prevents additional call to db in later validation
                    return Result.Failure<User>("School already have a headmaster, only one headmaster per school is allowed!");
            }

            Result<User> member = Create(firstName, lastName, email, role, gender, school);

            return member;
        }
    }
}
