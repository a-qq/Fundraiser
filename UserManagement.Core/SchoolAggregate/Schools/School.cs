using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.Utils;
using SchoolManagement.Core.SchoolAggregate.Groups;
using SchoolManagement.Core.SchoolAggregate.Schools.Events;
using SchoolManagement.Core.SchoolAggregate.Users;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SchoolManagement.Core.SchoolAggregate.Schools
{
    public class School : AggregateRoot<Guid>
    {
        private readonly List<User> _members = new List<User>();
        private readonly List<Group> _groups = new List<Group>();

        public Name Name { get; private set; }
        public Description Description { get; private set; }
        public string LogoId { get; private set; }
        public virtual IReadOnlyList<User> Members => _members.AsReadOnly();
        public virtual IReadOnlyList<Group> Groups => _groups.AsReadOnly();

        protected School()
        {
        }

        internal School(Name name, FirstName firstName, LastName lastName, Email email, Gender gender)
            : base(Guid.NewGuid())
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));

            Result<User> headmaster = EnrollCandidate(firstName, lastName, email, Role.Headmaster, gender);
            _members.Add(headmaster.Value);
        }

        internal void Edit(Name name, Description description)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            EditInfo(description);
        }

        internal void EditInfo(Description description)
        {
            this.Description = description ?? throw new ArgumentNullException(nameof(description));
        }

        internal void EditLogo()
        {
            this.LogoId = Guid.NewGuid().ToString();
        }

        internal Result<User> EnrollCandidate(FirstName firstName, LastName lastName, Email email, Role role, Gender gender)
        {
            User candidate = new User(firstName, lastName, email, role, gender, this);

            if (candidate.Role > Role.Headmaster)
                return Result.Failure<User>("Role out of school members' scope!");

            if (candidate.Role == Role.Headmaster && Members.Any(m => m.Role == Role.Headmaster))
                return Result.Failure<User>("School already have a headmaster, only one headmaster per school is allowed!");

            _members.Add(candidate);

            AddDomainEvent(new UserEnrolledEvent(candidate.Id, candidate.FirstName,
                candidate.LastName, candidate.Email, candidate.Role, candidate.Gender, Id));

            return Result.Success(candidate);
        }

        internal Result<Group> AddGroup(Number number, Sign sign)
        {
            if (Groups.Any(g => g.Code == number + sign))
                return Result.Failure<Group>($"Group with code {number + sign} already exist!");

             Group group = new Group(number, sign, this);
            _groups.Add(group);

            return Result.Success(group);
        }
    }
}
