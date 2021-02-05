using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.Utils;
using SchoolManagement.Core.SchoolAggregate.Groups;
using SchoolManagement.Core.SchoolAggregate.Members;
using SchoolManagement.Core.SchoolAggregate.Schools.Events;
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
        public YearsOfEducation YearsOfEducation { get; }
        public string LogoId { get; private set; }
        public virtual IReadOnlyList<Member> Members => _members.AsReadOnly();
        public virtual IReadOnlyList<Group> Groups => _groups.AsReadOnly();

        protected School()
        {
        }

        public School(Name name, YearsOfEducation yearsOfEducation, FirstName firstName, LastName lastName, Email email, Gender gender)
            : base(Guid.NewGuid())
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.YearsOfEducation = yearsOfEducation ?? throw new ArgumentNullException(nameof(yearsOfEducation));

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

            AddDomainEvent(new MemberEnrolledEvent(candidate.Id));

            return Result.Success(candidate);
        }

        public Result ExpellMember(Member member)
        {
            if (member.School != this)
                throw new InvalidOperationException(nameof(ExpellMember));

            if (member.Role == Role.Headmaster)
                return Result.Failure("Headmaster cannot be expelled!");

            //events of divestion are not raised, because member is deleted by member expelled event
            if (member.Role == Role.Teacher)
            {
                Maybe<Group> groupOrNone = this.Groups.TryFirst(g => g.FormTutor == member);
                if (groupOrNone.HasValue)
                    groupOrNone.Value.DivestFormTutor();
            }
            else if (member.Role == Role.Student)
            {
                Maybe<Group> groupOrNone = Maybe<Group>.From(member.Group);
                if (groupOrNone.HasValue)
                    groupOrNone.Value.DivestTreasurer();
            }

            if (!this._members.Remove(member))
                throw new InvalidOperationException(nameof(ExpellMember));

            AddDomainEvent(new MemberExpelledEvent(member.Id));

            return Result.Success();
        }

        public Result<Group> CreateGroup(Number number, Sign sign)
        {
            if (number == null)
                throw new ArgumentNullException(nameof(number));

            if (sign == null)
                throw new ArgumentNullException(nameof(sign));

            if (number > this.YearsOfEducation)
                return Result.Failure<Group>($"Number ('{number}') cannot be greater then years of education ('{YearsOfEducation}')!");

            if (Groups.Any(g => g.Code == number + sign))
                return Result.Failure<Group>($"Group with code {number + sign} already exist!");

            Group group = new Group(number, sign, this);
            _groups.Add(group);

            return Result.Success(group);
        }

        public Result<bool, Error> AssignMembersToGroup(Group group, IEnumerable<Member> members)
        {
            if (group == null)
                throw new ArgumentNullException(nameof(group));

            if (group.School != this)
                throw new InvalidOperationException(nameof(AssignMembersToGroup));

            Result<bool, Error> result = group.AssignMembers(members);
            return result;
        }

        public Result<bool, Error> ReassignStudentToGroup(Group group, Member member)
        {
            if (group == null)
                throw new ArgumentNullException(nameof(group));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            if (group.School != this)
                throw new InvalidOperationException(nameof(ReassignStudentToGroup));

            if (member.School != this)
                throw new InvalidOperationException(nameof(ReassignStudentToGroup));

            Maybe<Member> treasurerOrNone = Maybe<Member>.None;
            var memberAsEnumerable = member.Yield();

            if (member.Group != null) //prevalidate only if needed
            {
                Result validation = group.HaveSpaceFor(memberAsEnumerable);
                if (validation.IsFailure)
                    return Result.Failure<bool, Error>(new Error(validation.Error));

                treasurerOrNone = Maybe<Member>.From(member.Group.Treasurer);

                member.Group.DisenrollStudent(member);
            }

            var result = group.AssignMembers(memberAsEnumerable);

            if (result.IsSuccess && treasurerOrNone.HasValue && treasurerOrNone.Value == member)
                AddDomainEvent(new TreasurerDivestedEvent(member.Id));

            return result;
        }

        public Result MakeTeacherFormTutor(Member member, Group group)
        {
            if (group == null)
                throw new ArgumentNullException(nameof(group));

            if (group.School != this)
                throw new InvalidOperationException(nameof(MakeTeacherFormTutor));

            Maybe<Member> previousFormTutor = Maybe<Member>.From(group.FormTutor);

            Result result = group.AssignFormTutor(member);

            if (result.IsSuccess)
            {
                if (previousFormTutor.HasValue)
                    AddDomainEvent(new FormTutorDivestedEvent(previousFormTutor.Value.Id));

                AddDomainEvent(new FormTutorAssignedEvent(group.FormTutor.Id));
            }

            return result;
        }

        public Result DivestFormTutor(Group group)
        {
            if (group == null)
                throw new ArgumentNullException(nameof(group));

            if (group.School != this)
                throw new InvalidOperationException(nameof(MakeTeacherFormTutor));

            Maybe<Member> formTutor = Maybe<Member>.From(group.FormTutor);

            Result result = group.DivestFormTutor();
            if (result.IsSuccess && formTutor.HasValue)
                AddDomainEvent(new FormTutorDivestedEvent(formTutor.Value.Id));

            return result;
        }

        public Result PromoteTreasurer(Group group, Member student)
        {
            if (group == null)
                throw new ArgumentNullException(nameof(group));

            if (group.School != this)
                throw new InvalidOperationException(nameof(PromoteTreasurer));

            if (group.Treasurer == student)
                return Result.Failure($"'{student.Email.Value}'(Id: '{student.Id}') is already Treasurer of group '{group.Code}'!");

            if (group.Treasurer != null)
                DivestTreasurer(group);

            group.PromoteTreasurer(student);
            AddDomainEvent(new TreasurerPromotedEvent(student.Id));

            return Result.Success();
        }

        public Result DivestTreasurer(Group group)
        {
            if (group == null)
                throw new ArgumentNullException(nameof(group));

            if (group.School != this)
                throw new InvalidOperationException(nameof(DivestTreasurer));

            Maybe<Member> treasurer = Maybe<Member>.From(group.Treasurer);

            Result result = group.DivestTreasurer();
            if (result.IsSuccess && treasurer.HasValue)
                AddDomainEvent(new TreasurerDivestedEvent(treasurer.Value.Id));

            return result;
        }

        public void DisenrollStudentFromGroup(Group group, Member student)
        {
            if (group == null)
                throw new ArgumentNullException(nameof(group));

            if (group.School != this)
                throw new InvalidOperationException(nameof(DisenrollStudentFromGroup));

            if (group.Treasurer == student)
                AddDomainEvent(new TreasurerDivestedEvent(student.Id));

            group.DisenrollStudent(student);
        }

        public void DeleteGroup(Group group)
        {
            if (!group.IsArchived)
            {
                DivestFormTutor(group);
                DivestTreasurer(group);
            }

            group.DisenrollAllStudents();

            if (!_groups.Remove(group))
                throw new InvalidOperationException(nameof(DeleteGroup));
        }

        public Result<bool, Error> Graduate()
        {
            Result<bool, Error> validationResult = Result.Success<bool, Error>(true);
            foreach (var group in this.Groups)
                validationResult = Result.Combine(validationResult, group.CanGraduate());

            if (validationResult.IsFailure)
                return validationResult;

            List<Guid> membersToArchive = new List<Guid>();
            List<Guid> formTutorsToDivest = new List<Guid>();
            List<Guid> treasurersToDivest = new List<Guid>();           

            foreach (var group in this.Groups)
            {
                if (group.Number >= this.YearsOfEducation)
                {
                    membersToArchive.AddRange(group.Students.Select(m => m.Id));
                    if (group.FormTutor != null)
                        formTutorsToDivest.Add(group.FormTutor.Id);

                    if (group.Treasurer != null)
                        treasurersToDivest.Add(group.Treasurer.Id);
                }
                group.Graduate();
            }

            if (membersToArchive.Any() || formTutorsToDivest.Any() || treasurersToDivest.Any())
                AddDomainEvent(new GraduationCompletedEvent(membersToArchive, formTutorsToDivest, treasurersToDivest));

            return Result.Success<bool, Error>(true);
        }

        public void Remove()
        {
            ClearEvents();
            AddDomainEvent(new SchoolRemovedEvent(this.Id));
        }

        //validation&helper methods
        internal Result CanBeFormTutor(Member member)
        {
            if (member.School != this || member.IsArchived)
                throw new InvalidOperationException(nameof(CanBeFormTutor));

            if (member.Role != Role.Teacher)
                return Result.Failure($"'{member.Email.Value}'(Id: '{member.Id}') is not a {Role.Teacher}!");

            Maybe<Group> groupOrNone = this.Groups.TryFirst(g => g.FormTutor == member);
            if (groupOrNone.HasValue)
                return Result.Failure($"'{member.Email.Value}'(Id: '{member.Id}') is already form tutor of group '{groupOrNone.Value.Code}'!");

            return Result.Success();
        }

        public void MergeEnrollmentEvents()
        {
            var eventsToMerge = DomainEvents.OfType<MemberEnrolledEvent>().ToList();
            var otherEvents = DomainEvents.Except(eventsToMerge).ToList();

            ClearEvents();

            IEnumerable<Guid> memberIds = eventsToMerge.Select(e => e.MemberId);
            var bulkEvent = new MembersEnrolledEvent(memberIds);

            AddDomainEvent(bulkEvent);
            if (otherEvents.Any())
                foreach (var other in otherEvents)
                    AddDomainEvent(other);
        }
    }
}
