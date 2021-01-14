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
            Name = name ?? throw new ArgumentNullException(nameof(name));

            var headmaster = User.Create(firstName, lastName, email, Role.Headmaster, gender, this);
            _members.Add(headmaster.Value);
        }

        internal Result Enroll(User candidate)
        {
            if (candidate == null)
                throw new ArgumentNullException(nameof(candidate));

            if (candidate.Role > Role.Headmaster)
                return Result.Failure("Role out of school members' scope!");

            if (candidate.Role == Role.Headmaster && Members.Any(m => m.Role == Role.Headmaster))
                return Result.Failure("School already have a headmaster, only one headmaster per school is allowed!");

            _members.Add(candidate);

            AddDomainEvent(new UserEnrolledEvent(candidate.Id, candidate.FirstName,
                candidate.LastName, candidate.Email, candidate.Role, candidate.Gender, Id));

            return Result.Success();
        }
    }
}
