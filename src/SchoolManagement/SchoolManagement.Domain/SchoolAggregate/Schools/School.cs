using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using SchoolManagement.Domain.SchoolAggregate.Groups;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SchoolManagement.Domain.SchoolAggregate.Schools.Events;
using SharedKernel.Domain.Common;
using SharedKernel.Domain.Errors;
using SharedKernel.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SchoolManagement.Domain.SchoolAggregate.Schools
{
    public class School : AggregateRoot<SchoolId>
    {
        private readonly List<Group> _groups = new List<Group>();
        private readonly List<Member> _members = new List<Member>();

        protected School()
        {
        }

        public School(Name name, YearsOfEducation yearsOfEducation, FirstName firstName, LastName lastName, Email email,
            Gender gender)
            : base(SchoolId.New())
        {
            Name = Guard.Against.Null(name, nameof(name));
            YearsOfEducation = Guard.Against.Null(yearsOfEducation, nameof(yearsOfEducation));

            EnrollCandidate(firstName, lastName, email, Role.Headmaster, gender);
            AddDomainEvent(new SchoolCreatedDomainEvent(Id));
        }

        public Name Name { get; private set; }
        public Description Description { get; private set; }
        public GroupMembersLimit GroupMembersLimit { get; private set; }
        public YearsOfEducation YearsOfEducation { get; }
        public string LogoId { get; private set; }
        public virtual IReadOnlyList<Member> Members => _members.AsReadOnly();
        public virtual IReadOnlyList<Group> Groups => _groups.AsReadOnly();

        public Result<bool, Error> Edit(Name name, Description description, GroupMembersLimit limit)
        {
            Name = Guard.Against.Null(name, nameof(name));
            return EditInfo(description, limit);
        }

        public Result<bool, Error> EditInfo(Description description, GroupMembersLimit limit)
        {
            Description = Guard.Against.Null(description, nameof(description));
            Guard.Against.Null(limit, nameof(limit));

            var result = Result.Success<bool, Error>(true);
            if (limit != null)
            {
                _ = this.Members; //load members
                var groupsOverLimit = this.Groups.Where(g => g.Students.Count > limit);

                foreach (var group in groupsOverLimit)
                {
                    result = Result.Combine(result, Result.Failure<bool, Error>(new Error(
                        $"Group '{group.Code}' (Id: '{group.Id}') has more then " +
                        $"'{limit}' student(s)! (Currently: '{group.Students.Count}')")));
                }
            }

            if (result.IsSuccess)
                GroupMembersLimit = limit;

            return result;
        }

        public void EditLogo()
        {
            LogoId = Guid.NewGuid().ToString();
        }

        public Result<Member, Error> EnrollCandidate(
            FirstName firstName, LastName lastName, Email email, Role role, Gender gender)
        {
            var candidate = new Member(firstName, lastName, email, role, gender, this);

            if (candidate.Role == Role.Headmaster)
            {
                var validation = CanPromoteHeadmaster();
                if (validation.IsFailure)
                    return validation.Error;
            }

            _members.Add(candidate);

            AddDomainEvent(new MemberEnrolledDomainEvent(this.Id, candidate.Id));

            return candidate;
        }

        internal Result<bool, Error> CanPromoteHeadmaster()
        {
            var headmaster = Members.TryFirst(m => m.Role == Role.Headmaster);
            if (headmaster.HasValue)
                return new Error(
                    $"School already have a headmaster: '{headmaster.Value.Email}' (Id: '{headmaster.Value.Id}')");

            return Result.Success<bool, Error>(true);
        }

        public Result<bool, Error> ExpelMember(MemberId memberId)
        {
            var member = _members.Single(m => m.Id == memberId);

            if (member.Role == Role.Headmaster)
                return new Error($"Headmaster '{member.Email}' (Id: '{member.Id}' cannot be expelled!");

            if (member.Role == Role.Teacher)
            {
                var groups = AllGroupsOfFormTutor(member);
                foreach (var group in groups)
                    group.DivestFormTutor();
            }
            else //student
            {
                Maybe<Group> groupOrNone = member.Group;
                if (groupOrNone.HasValue)
                    groupOrNone.Value.DisenrollStudent(memberId);
            }

            _members.Remove(member);

            AddDomainEvent(new MemberExpelledDomainEvent(member.Id, member.IsActive));

            return Result.Success<bool, Error>(true);
        }

        public Result<bool, Error> ArchiveMember(MemberId memberId)
        {
            var member = _members.Single(m => m.Id == memberId);

            var result = member.Archive();

            if (result.IsFailure)
                return result.ConvertFailure<bool>();

            AddDomainEvent(new MemberArchivedDomainEvent(memberId, result.Value));

            return Result.Success<bool, Error>(true);
        }

        public Result<bool, Error> RestoreMember(MemberId memberId)
        {
            var member = _members.Single(m => m.Id == memberId);

            var result = member.Restore();

            if (result.IsSuccess)
                AddDomainEvent(new MemberRestoredDomainEvent(memberId));

            return result;
        }

        public Result<Group, Error> CreateGroup(Number number, Sign sign)
        {
            Guard.Against.Null(number, nameof(number));
            Guard.Against.Null(sign, nameof(sign));

            if (number > YearsOfEducation)
                return new Error($"Number ('{number}') cannot be greater then" +
                                 $" school's years of education ('{YearsOfEducation}')!");

            //lazy loading intentional
            if (Groups.Any(g => string.Equals(number + sign, g.Code, StringComparison.OrdinalIgnoreCase) && !g.IsArchived))
                return new Error($"Group with code {number + sign} already exist!");

            var group = new Group(number, sign, this);

            _groups.Add(group);

            return group;
        }

        public Result<Member, Error> AssignStudentToGroup(MemberId memberId, Code code)
        {
            var group = _groups.Single(g => g.Code == code && !g.IsArchived);

            var result = group.AssignStudent(memberId);
            if (result.IsSuccess)
                AddDomainEvent(new StudentAssignedDomainEvent(memberId, group.Id, result.Value.IsActive));

            return result;
        }

        public Result<bool, Error> PromoteFormTutor(GroupId groupId, MemberId memberId)
        {
            var group = _groups.Single(g => g.Id == groupId && !g.IsArchived);

            var result = group.AssignFormTutor(memberId);

            if (result.IsSuccess)
                AddDomainEvent(new FormTutorAssignedDomainEvent(group.FormTutor.Id, group.Id, group.FormTutor.IsActive));

            return result;
        }

        public void DivestFormTutorFromGroup(GroupId groupId)
        {
            var group = Groups.Single(g => g.Id == groupId && !g.IsArchived);

            var formTutor = group.DivestFormTutor();

            AddDomainEvent(new FormTutorDivestedDomainEvent(formTutor.Id, formTutor.IsActive));
        }

        public Result<bool, Error> PromoteTreasurer(GroupId groupId, MemberId studentId)
        {
            var group = _groups.Single(g => g.Id == groupId && !g.IsArchived);

            var result = group.PromoteTreasurer(studentId);

            if (result.IsSuccess)
                AddDomainEvent(new TreasurerPromotedDomainEvent(group.Treasurer.Id, group.Treasurer.IsActive));

            return result;
        }

        public void DivestTreasurerFromGroup(GroupId groupId)
        {
            var group = Groups.Single(g => g.Id == groupId && !g.IsArchived);

            var treasurer = group.DivestTreasurer();

            AddDomainEvent(new TreasurerDivestedDomainEvent(treasurer.Id, treasurer.IsActive));
        }

        public void DisenrollStudentFromGroup(GroupId groupId, MemberId studentId)
        {
            var group = _groups.Single(g => g.Id == groupId && !g.IsArchived);

            var (student, removedRole) = group.DisenrollStudent(studentId);

            AddDomainEvent(new StudentDisenrolledDomainEvent(student.Id, removedRole, student.IsActive));
        }

        public void DivestHeadmaster()
        {
            var headmaster = _members.Single(
                m => m.Role == Role.Headmaster && !m.IsArchived);

            headmaster.DegradeToTeacher();

            AddDomainEvent(new HeadmasterDivestedDomainEvent(headmaster.Id, headmaster.IsActive));
        }

        public Result<bool, Error> PromoteHeadmaster(MemberId memberId)
        {
            var teacher = _members.Single(
                m => m.Id == memberId && m.Role == Role.Teacher && !m.IsArchived);

            var result = teacher.PromoteToHeadmaster();
            if (result.IsSuccess)
                AddDomainEvent(new HeadmasterPromotedDomainEvent(teacher.Id, teacher.IsActive));

            return result;
        }

        public void PassOnHeadmaster(MemberId memberId)
        {
            var headmaster = _members.Single(
                m => m.Role == Role.Headmaster && !m.IsArchived);

            var teacher = _members.Single(
                m => m.Id == memberId && m.Role == Role.Teacher && !m.IsArchived);

            headmaster.DegradeToTeacher();

            if (teacher.PromoteToHeadmaster().IsFailure)
                throw new InvalidOperationException(nameof(School) + ":" + nameof(PassOnHeadmaster));

            AddDomainEvent(new HeadmasterDivestedDomainEvent(headmaster.Id, headmaster.IsActive));
            AddDomainEvent(new HeadmasterPromotedDomainEvent(teacher.Id, teacher.IsActive));
        }

        public void DeleteGroup(GroupId groupId)
        {
            var group = _groups.Single(g => g.Id == groupId);

            group.Delete();
            //TODO: Add Group deleted event
            _groups.Remove(group);
        }

        public Result<bool, Error> Graduate()
        {
            var groupsToGraduate = Groups.Where(g => !g.IsArchived);
            var result = Result.Success<bool, Error>(true);
            foreach (var group in groupsToGraduate)
            {
                result = Result.Combine(result, group.Graduate());

                if (!group.IsArchived)
                    continue;

                if (!(group.FormTutor is null))
                    AddDomainEvent(new FormTutorDivestedDomainEvent(group.FormTutor.Id, group.FormTutor.IsActive));
            }

            return result;
        }

        public Result MarkMemberAsActive(MemberId memberId)
        {
            var member = _members.Single(m => m.Id == memberId);

            var result = member.MarkAsActive();

            if (result.IsSuccess)
                AddDomainEvent(new MemberActivatedDomainEvent(member.School.Id, member.Id));
            
            return result;
        }

        public Maybe<Group> CurrentGroupOfFormTutor(Member member)
        {
            Guard.Against.Null(member, nameof(member));

            return member.Role == Role.Student
                ? Maybe<Group>.None
                : Groups.SingleOrDefault(g => !g.IsArchived && g.FormTutor == member);
        }

        internal IEnumerable<Group> AllGroupsOfFormTutor(Member member)
        {
            Guard.Against.Null(member, nameof(member));

            return member.Role != Role.Teacher
                ? Enumerable.Empty<Group>()
                : Groups.Where(g => g.FormTutor == member);
        }
    }
}