using CSharpFunctionalExtensions;
using SchoolManagement.Domain.SchoolAggregate.Groups;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Domain.Errors;
using SharedKernel.Domain.ValueObjects;
using System;

namespace SchoolManagement.Domain.SchoolAggregate.Members
{
    public class Member : Entity<MemberId>
    {
        public FirstName FirstName { get; }
        public LastName LastName { get; }
        public Role Role { get; private set; }
        public Email Email { get; }
        public Gender Gender { get; }
        public bool IsActive { get; }
        public bool IsArchived { get; private set; }
        public virtual School School { get; }
        public virtual Group Group { get; private set; }

        protected Member() { }

        internal Member(FirstName firstName, LastName lastName, Email email, Role role, Gender gender, School school)
            : base(MemberId.New())
        {
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            Role = role ?? throw new ArgumentNullException(nameof(role));
            Gender = gender ?? throw new ArgumentNullException(nameof(gender));
            School = school ?? throw new ArgumentNullException(nameof(school));
            IsActive = false;
            IsArchived = false;
        }

        internal Result<bool, Error> Archive()
        {
            if (this.IsArchived)
                throw new InvalidOperationException(nameof(Member) + ":" + nameof(Archive));

            var validation = CanBeArchived();

            if (validation.IsFailure)
                return validation;

            var groupOrNone = this.School.GroupOfFormTutor(this);

            if (groupOrNone.HasValue)
                this.School.DivestFormTutorFromGroup(groupOrNone.Value.Id);

            this.IsArchived = true;

            return Result.Success<bool, Error>(true);
        }

        internal Result<bool, Error> CanBeArchived()
        {
            if (this.IsArchived)
                throw new InvalidOperationException(nameof(Member) + ":" + nameof(CanBeArchived));

            var result = Result.Combine(
                 Result.FailureIf(this.Role == Role.Headmaster, true,
                    new Error($"Headmaster '{this.Email}' (Id: '{this.Id}') cannot be archived!")),
                 Result.FailureIf(!this.IsActive, true,
                    new Error($"Cannot archive not active member '{this.Email}' (Id: '{this.Id}')!")));

            return result;
        }

        internal Result<bool, Error> Restore()
        {
            if (!this.IsArchived)
                return new Error($"Member '{this.Email}' (Id: '{this.Id}') is not archived!");

            if (this.Role == Role.Student)
            {
                Maybe<Group> groupOrNone = this.Group;
                if (groupOrNone.HasValue)
                    groupOrNone.Value.DisenrollStudent(this.Id);
            }

            this.IsArchived = false;

            return Result.Success<bool, Error>(true);
        }

        internal void DegradeToTeacher()
        {
            if (this.Role != Role.Headmaster || this.IsArchived)
                throw new InvalidOperationException(nameof(Member) + ":" + nameof(DegradeToTeacher));

            this.Role = Role.Teacher;
        }

        internal Result<bool, Error> PromoteToHeadmaster()
        {
            if (this.Role != Role.Teacher || this.IsArchived)
                throw new InvalidOperationException(nameof(Member) + ":" + nameof(PromoteToHeadmaster));

            var validation = this.School.CanPromoteHeadmaster();
            if (validation.IsFailure)
                return validation;

            this.Role = Role.Headmaster;

            return Result.Success<bool, Error>(true);
        }
    }
}
