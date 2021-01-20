using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.Utils;
using SchoolManagement.Core.SchoolAggregate.Groups;
using SchoolManagement.Core.SchoolAggregate.Schools.Events;
using SchoolManagement.Core.SchoolAggregate.Members;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SchoolManagement.Core.SchoolAggregate.Schools
{
    public class School : AggregateRoot<Guid>
    {
        private readonly List<Member> _members = new List<Member>();
        private readonly List<Group> _groups = new List<Group>();

        public Name Name { get; private set; }
        public Description Description { get; private set; }
        public GroupMembersLimit GroupMembersLimit { get; private set; }
        public string LogoId { get; private set; }
        public virtual IReadOnlyList<Member> Members => _members.AsReadOnly();
        public virtual IReadOnlyList<Group> Groups => _groups.AsReadOnly();

        protected School()
        {
        }

        public School(Name name, FirstName firstName, LastName lastName, Email email, Gender gender)
            : base(Guid.NewGuid())
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));

            Result<Member> headmaster = EnrollCandidate(firstName, lastName, email, Role.Headmaster, gender);
            _members.Add(headmaster.Value);
        }

        public void Edit(Name name, Description description, GroupMembersLimit limit)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            EditInfo(description, limit);
        }

        public void EditInfo(Description description, GroupMembersLimit groupMembersLimit)
        {
            this.Description = description ?? throw new ArgumentNullException(nameof(description));
            this.GroupMembersLimit = groupMembersLimit ?? throw new ArgumentNullException(nameof(groupMembersLimit));
        }

        public void EditLogo()
        {
            this.LogoId = Guid.NewGuid().ToString();
        }

        public Result<Member> EnrollCandidate(FirstName firstName, LastName lastName, Email email, Role role, Gender gender)
        {
            Member candidate = new Member(firstName, lastName, email, role, gender, this);

            if (candidate.Role == Role.Headmaster && Members.Any(m => m.Role == Role.Headmaster))
                return Result.Failure<Member>("School already have a headmaster, only one headmaster per school is allowed!");

            _members.Add(candidate);

            AddDomainEvent(new MemberEnrolledEvent(candidate.Id, candidate.FirstName,
                candidate.LastName, candidate.Email, candidate.Role, candidate.Gender, Id));

            return Result.Success(candidate);
        }

        public Result<Group> CreateGroup(Number number, Sign sign)
        {
            if (Groups.Any(g => g.Code == number + sign))
                return Result.Failure<Group>($"Group with code {number + sign} already exist!");

            Group group = new Group(number, sign, this);
            _groups.Add(group);

            return Result.Success(group);
        }

        public Result<bool, Error> AssignMembersToGroup(Group group, IList<Member> members)
        {
            if (group.School != this)
                throw new InvalidOperationException(nameof(AssignMembersToGroup));

            Result<bool, Error> result = group.AssignMembers(members);
            return result;
        }
    }
}
