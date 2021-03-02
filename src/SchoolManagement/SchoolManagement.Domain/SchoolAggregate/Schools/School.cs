using CSharpFunctionalExtensions;
using SchoolManagement.Domain.SchoolAggregate.Groups;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SchoolManagement.Domain.SchoolAggregate.Schools.Events;
using SharedKernel.Domain.Common;
using SharedKernel.Domain.Errors;
using SharedKernel.Domain.Extensions;
using SharedKernel.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SchoolManagement.Domain.SchoolAggregate.Schools
{
    public class School : AggregateRoot<SchoolId>
    {
        private readonly List<Member> _members = new List<Member>();
        private readonly List<Group> _groups = new List<Group>();

        public Name Name { get; private set; }
        public Description Description { get; private set; }
        public GroupMembersLimit GroupMembersLimit { get; private set; }
        public YearsOfEducation YearsOfEducation { get; }
        public string LogoId { get; private set; }
        public virtual IReadOnlyList<Member> Members => _members.AsReadOnly();
        public virtual IReadOnlyList<Group> Groups => _groups.AsReadOnly();

        protected School() { }

        public School(Name name, YearsOfEducation yearsOfEducation, FirstName firstName, LastName lastName, Email email, Gender gender)
            : base(SchoolId.New())
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.YearsOfEducation = yearsOfEducation ?? throw new ArgumentNullException(nameof(yearsOfEducation));

            var headmaster = EnrollCandidate(firstName, lastName, email, Role.Headmaster, gender).Value;
            _members.Add(headmaster);
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

        public Result<Member, Error> EnrollCandidate(
            FirstName firstName, LastName lastName, Email email, Role role, Gender gender)
        {
            Member candidate = new Member(firstName, lastName, email, role, gender, this);

            if (candidate.Role == Role.Headmaster)
            {
                var validation = CanPromoteHeadmaster();
                if (validation.IsFailure)
                    return validation.Error;
            }

            _members.Add(candidate);

            AddDomainEvent(new MemberEnrolledEvent(candidate.Id));

            return candidate;
        }

        public Result<IEnumerable<Member>, Error> EnrollCandidates(
            IEnumerable<MemberEnrollmentAssignmentData> membersData)
        {
            if (membersData is null || !membersData.Any())
                throw new ArgumentException(nameof(membersData));

            List<Member> addedMembers = new List<Member>();
            var validation = Result.Success<bool, Error>(true);

            if (membersData.Any(m => m.Role == Role.Headmaster))
                validation = CanPromoteHeadmaster();

            var membersGroups = membersData.ToLookup(x => x.GroupCode.HasValue);
            if (membersGroups.Contains(true))
            {
                var studentGroups = membersGroups[true].GroupBy(s => s.GroupCode.Value).ToArray();
                List<Group> groups = new List<Group>();
                foreach (var studentGroup in studentGroups)
                {
                    var group = this.Groups.Single(g => g.Code == studentGroup.Key && !g.IsArchived);
                    validation = Result.Combine(validation, group.HaveSpaceFor(studentGroup.Count()));

                    foreach (var student in studentGroup)
                    {
                        Result.Combine(validation, Result.FailureIf(student.Role != Role.Student, true,
                            new Error($"Candidate '{student.Email}' is not a '{Role.Student}'!")));
                    }

                    groups.Add(group);
                }

                if (validation.IsFailure)
                    return validation.Error;

                for (int i = 0; i < groups.Count; i++)
                {
                    List<Member> addedStudents = new List<Member>();
                    foreach (var candidate in studentGroups[i])
                    {
                        Member student = new Member(candidate.FirstName, candidate.LastName,
                            candidate.Email, candidate.Role, candidate.Gender, this);

                        _members.Add(student);
                        addedStudents.Add(student);
                    }

                    if (groups[i].AssignStudents(addedStudents.Select(s => s.Id)).IsFailure)
                        throw new InvalidOperationException(nameof(School) + ":" + nameof(EnrollCandidates));

                    addedMembers.AddRange(addedStudents);
                }
            }

            if (membersGroups.Contains(false))
            {
                foreach (var candidate in membersGroups[false])
                {
                    Member student = new Member(candidate.FirstName, candidate.LastName,
                         candidate.Email, candidate.Role, candidate.Gender, this);

                    _members.Add(student);
                    addedMembers.Add(student);
                }
            }

            AddDomainEvent(new MembersEnrolledEvent(addedMembers.Select(m => m.Id)));

            return addedMembers;
        }

        internal Result<bool, Error> CanPromoteHeadmaster()
        {
            var headmaster = Members.TryFirst(m => m.Role == Role.Headmaster);
            if (headmaster.HasValue)
                return new Error($"School already have a headmaster: '{headmaster.Value.Email}'(Id: '{headmaster.Value.Id}')");

            return Result.Success<bool, Error>(true);
        }

        public Result<bool, Error> ExpellMember(MemberId memberId)
        {
            Member member = this._members.Single(m => m.Id == memberId);

            if (member.Role == Role.Headmaster)
                return new Error($"Headmaster '{member.Email}'(Id: '{member.Id} cannot be expelled!");

            //events of divestion are not raised, because member is deleted in other context by member expelled event
            if (member.Role == Role.Teacher)
            {
                var groupsOrNone = AllGroupsOfFormTutor(member);
                if (groupsOrNone.HasValue)
                    foreach (var group in groupsOrNone.Value)
                    {
                        if (group.IsArchived)
                            group.DivestFormTutor();

                        else DivestFormTutorFromGroup(group.Id);
                    }
            }
            else //student
            {
                Maybe<Group> groupOrNone = member.Group;
                if (groupOrNone.HasValue)
                    groupOrNone.Value.DisenrollStudent(memberId);

            }

            this._members.Remove(member);

            AddDomainEvent(new MemberExpelledEvent(member.Id));

            return Result.Success<bool, Error>(true);
        }

        public Result<bool, Error> ArchiveMember(MemberId memberId)
        {
            Member member = this.Members.Single(m => m.Id == memberId && !m.IsArchived);

            var result = member.Archive();

            if (result.IsSuccess)
                AddDomainEvent(new MemberArchivedEvent(memberId));

            return result;
        }

        public Result<bool, Error> RestoreMember(MemberId memberId)
        {
            Member member = this.Members.Single(m => m.Id == memberId);

            var result = member.Restore();

            if (result.IsSuccess)
                AddDomainEvent(new MemberRestoredEvent(memberId));

            return result;
        }

        public Result<Group, Error> CreateGroup(Number number, Sign sign)
        {
            if (number == null)
                throw new ArgumentNullException(nameof(number));

            if (sign == null)
                throw new ArgumentNullException(nameof(sign));

            if (number > this.YearsOfEducation)
                return new Error($"Number ('{number}') cannot be greater then" +
                    $" school's years of education ('{this.YearsOfEducation}')!");

            //lazy load intentional
            if (this.Groups.Any(g => string.Equals(number + sign, g.Code, StringComparison.OrdinalIgnoreCase)))
                return new Error($"Group with code {number + sign} already exist!");

            Group group = new Group(number, sign, this);

            _groups.Add(group);

            return group;
        }

        public Result<bool, Error> AssignStudentsToGroup(GroupId groupId, IEnumerable<MemberId> memberIds)
        {
            Group group = this.Groups.Single(g => g.Id == groupId && !g.IsArchived);

            var result = group.AssignStudents(memberIds);
            if (result.IsSuccess)
                AddDomainEvent(new StudentsAssignedEvent(groupId, memberIds.Distinct()));
            return result;
        }

        public Result<bool, Error> ReassignStudentToGroup(GroupId groupId, MemberId memberId)
        {
            Group group = this._groups.Single(g => g.Id == groupId && !g.IsArchived);

            Member member = this.Members.Single(
                m => m.Id == memberId && !m.IsArchived && m.Role == Role.Student);

            Maybe<Group> previousGroupOrNone = member.Group;

            if (previousGroupOrNone.HasValue) //prevalidate only if needed
            {
                var validation = group.HaveSpaceFor(member.Yield());

                if (validation.IsFailure)
                    return validation;

                DisenrollStudentFromGroup(previousGroupOrNone.Value.Id, member.Id);
            }

            if (group.AssignStudents(memberId.Yield()).IsFailure)
                throw new InvalidOperationException(nameof(School) + ":" + nameof(ReassignStudentToGroup));

            return Result.Success<bool, Error>(true);
        }

        public Result<bool, Error> PromoteFormTutor(GroupId groupId, MemberId memberId)
        {
            Group group = this._groups.Single(g => g.Id == groupId && !g.IsArchived);

            var result = group.AssignFormTutor(memberId);

            if (result.IsSuccess)
                AddDomainEvent(new FormTutorAssignedEvent(group.FormTutor.Id, group.Id));

            return result;
        }

        public void DivestFormTutorFromGroup(GroupId groupId)
        {
            Group group = this.Groups.Single(g => g.Id == groupId && !g.IsArchived);

            var formTutorId = group.DivestFormTutor();

            AddDomainEvent(new FormTutorDivestedEvent(formTutorId));
        }

        public Result<bool, Error> PromoteTreasurer(GroupId groupId, MemberId studentId)
        {
            Group group = this._groups.Single(g => g.Id == groupId && !g.IsArchived);

            var result = group.PromoteTreasurer(studentId);

            if (result.IsSuccess)
                AddDomainEvent(new TreasurerPromotedEvent(group.Treasurer.Id));

            return result;
        }

        public void DivestTreasurerFromGroup(GroupId groupId)
        {
            Group group = this.Groups.Single(g => g.Id == groupId && !g.IsArchived);

            var treasurerId = group.DivestTreasurer();

            AddDomainEvent(new TreasurerDivestedEvent(treasurerId));
        }

        public void DisenrollStudentFromGroup(GroupId groupId, MemberId studentId)
        {
            Group group = this._groups.Single(g => g.Id == groupId && !g.IsArchived);

            group.DisenrollStudent(studentId);

            AddDomainEvent(new StudentDisenrolledFromGroupEvent(studentId));
        }

        public void DivestHeadmaster()
        {
            Member headmaster = this._members.Single(
                m => m.Role == Role.Headmaster && !m.IsArchived);

            headmaster.DegradeToTeacher();

            AddDomainEvent(new HeadmasterDivestedEvent(headmaster.Id));
        }

        public Result<bool, Error> PromoteHeadmaster(MemberId memberId)
        {
            Member teacher = this._members.Single(
                m => m.Id == memberId && m.Role == Role.Teacher && !m.IsArchived);

            var result = teacher.PromoteToHeadmaster();
            if (result.IsSuccess)
                AddDomainEvent(new HeadmasterPromotedEvent(teacher.Id));

            return result;
        }

        public void PassOnHeadmaster(MemberId memberId)
        {
            Member headmaster = this._members.Single(
                m => m.Role == Role.Headmaster && !m.IsArchived);

            Member teacher = this._members.Single(
                m => m.Id == memberId && m.Role == Role.Teacher && !m.IsArchived);

            headmaster.DegradeToTeacher();

            if (teacher.PromoteToHeadmaster().IsFailure)
                throw new InvalidOperationException(nameof(School) + ":" + nameof(PassOnHeadmaster));

            AddDomainEvent(new HeadmasterDivestedEvent(headmaster.Id));
            AddDomainEvent(new HeadmasterPromotedEvent(teacher.Id));
        }

        public void DeleteGroup(GroupId groupId)
        {
            Group group = this._groups.Single(g => g.Id == groupId);

            group.Delete();

            _groups.Remove(group);
        }

        public Result<bool, Error> Graduate()
        {
            Result<bool, Error> validation = Result.Success<bool, Error>(true);
            var groupsToGraduate = this._groups.TakeWhile(g => !g.IsArchived);

            foreach (var group in groupsToGraduate)
                validation = Result.Combine(validation, group.CanGraduate());

            if (validation.IsFailure)
                return validation;

            List<MemberId> studentIds = new List<MemberId>();
            List<MemberId> treasurerIds = new List<MemberId>();
            List<MemberId> formTutorIds = new List<MemberId>();

            foreach (var group in groupsToGraduate)
            {
                if (group.Number >= this.YearsOfEducation)
                {
                    group.Graduate();

                    studentIds.AddRange(group.Students.Select(m => m.Id));
                    Maybe<Member> treasurer = group.Treasurer;
                    if (treasurer.HasValue)
                        treasurerIds.Add(group.Treasurer.Id);

                    Maybe<Member> formTutor = group.FormTutor;
                    if (formTutor.HasValue)
                        treasurerIds.Add(group.FormTutor.Id);
                }
            }

            if (studentIds.Any() || treasurerIds.Any() || formTutorIds.Any())
                AddDomainEvent(new GraduationCompletedEvent(studentIds, treasurerIds, formTutorIds));

            return Result.Success<bool, Error>(true);
        }

        internal Maybe<Group> GroupOfFormTutor(Member member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            if (member.Role != Role.Teacher)
                return Maybe<Group>.None;

            return this.Groups.TryFirst(g => g.FormTutor == member && !g.IsArchived);
        }

        /// <summary>
        /// Looks for all <see cref="Group"/>s (including archived) with given <paramref name="member"/> as <see cref="Group.FormTutor"/>.</summary>
        /// <param name="member">
        ///     Member against which the search is conducted. </param>
        /// <returns> 
        ///     A Maybe&lt;IEnumerable&lt;<see cref="Group"/>&gt;&gt with all found groups or <see cref="Maybe.None"/>, if <paramref name="member"/> is not a teacher.</returns>

        public Maybe<IEnumerable<Group>> AllGroupsOfFormTutor(Member member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            if (member.Role != Role.Teacher)
                return Maybe<IEnumerable<Group>>.None;

            return Maybe<IEnumerable<Group>>.From(
                this.Groups.TakeWhile(g => g.FormTutor == member));
        }
    }
}
