using System;
using CSharpFunctionalExtensions;
using SchoolManagement.Domain.SchoolAggregate.Groups;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Domain.Errors;
using SharedKernel.Domain.ValueObjects;

namespace SchoolManagement.Domain.SchoolAggregate.Members
{
    public class Member : Entity<MemberId>
    {
        protected Member()
        {
        }

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

        public FirstName FirstName { get; }
        public LastName LastName { get; }
        public Role Role { get; private set; }
        public Email Email { get; }
        public Gender Gender { get; }
        public bool IsActive { get; }
        public bool IsArchived { get; private set; }
        public virtual School School { get; }
        public virtual Group Group { get; private set; }

        internal Result<bool, Error> Archive()
        {
            if (IsArchived)
                throw new InvalidOperationException(nameof(Member) + ":" + nameof(Archive));

            var validation = CanBeArchived();

            if (validation.IsFailure)
                return validation;

            var groupOrNone = School.GroupOfFormTutor(this);

            if (groupOrNone.HasValue)
                School.DivestFormTutorFromGroup(groupOrNone.Value.Id);

            IsArchived = true;

            return Result.Success<bool, Error>(true);
        }

        internal Result<bool, Error> CanBeArchived()
        {
            if (IsArchived)
                throw new InvalidOperationException(nameof(Member) + ":" + nameof(CanBeArchived));

            var result = Result.Combine(
                Result.FailureIf(Role == Role.Headmaster, true,
                    new Error($"Headmaster '{Email}' (Id: '{Id}') cannot be archived!")),
                Result.FailureIf(!IsActive, true,
                    new Error($"Cannot archive not active member '{Email}' (Id: '{Id}')!")));

            return result;
        }

        internal Result<bool, Error> Restore()
        {
            if (!IsArchived)
                return new Error($"Member '{Email}' (Id: '{Id}') is not archived!");

            if (Role == Role.Student)
            {
                Maybe<Group> groupOrNone = Group;
                if (groupOrNone.HasValue)
                    groupOrNone.Value.DisenrollStudent(Id);
            }

            IsArchived = false;

            return Result.Success<bool, Error>(true);
        }

        internal void DegradeToTeacher()
        {
            if (Role != Role.Headmaster || IsArchived)
                throw new InvalidOperationException(nameof(Member) + ":" + nameof(DegradeToTeacher));

            Role = Role.Teacher;
        }

        internal Result<bool, Error> PromoteToHeadmaster()
        {
            if (Role != Role.Teacher || IsArchived)
                throw new InvalidOperationException(nameof(Member) + ":" + nameof(PromoteToHeadmaster));

            var validation = School.CanPromoteHeadmaster();
            if (validation.IsFailure)
                return validation;

            Role = Role.Headmaster;

            return Result.Success<bool, Error>(true);
        }
    }
}