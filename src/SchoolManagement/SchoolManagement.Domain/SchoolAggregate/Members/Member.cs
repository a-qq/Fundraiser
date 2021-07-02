using System;
using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using SchoolManagement.Domain.SchoolAggregate.Groups;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Domain.Constants;
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
        public bool IsActive { get; private set; }
        public bool IsArchived { get; private set; }
        public virtual School School { get; }
        public virtual Group Group { get; private set; }

        internal Result<string, Error> Archive()
        {
            if (IsArchived)
                return new Error($"{Role} '{Email}' (Id: '{Id}') is already archived!");

            if (Role == Role.Headmaster)
                return new Error($"Headmaster '{Email}' (Id: '{Id}') cannot be archived!");

            if (!IsActive)
                return new Error($"Cannot archive not active member '{Email}' (Id: '{Id}')!");

            string groupRole = null;
            var groupOrNone = School.CurrentGroupOfFormTutor(this);

            if (groupOrNone.HasValue)
            {
                groupOrNone.Value.DivestFormTutor();
                groupRole = GroupRoles.FormTutor;
            }
            else if (!(Group is null) && !Group.IsArchived)
                groupRole = Group.DisenrollStudent(this.Id).Item2;
            else if (!(Group is null) && Group.IsArchived && Group.Treasurer == this)
                groupRole = GroupRoles.Treasurer;

            IsArchived = true;

            return Result.Success<string, Error>(groupRole);
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
            //event?!

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
            //event?!
            return Result.Success<bool, Error>(true);
        }

        internal Result MarkAsActive()
        {
            if (this.IsActive)
                return Result.Failure($"Member '{Id}' is already active!");

            IsActive = true;

            return Result.Success();
        }

        internal void SetGroup(Group group)
        {
            Guard.Against.Null(group, nameof(group));

            if (!(Group is null) || group.School != this.School)
                throw new InvalidOperationException(nameof(Member) + " : " + nameof(SetGroup));

            Group = group;
        }

        internal void RemoveFromGroup()
        {
            Group = null;
        }
    }
}