using System;
using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Domain.Errors;

namespace SchoolManagement.Domain.SchoolAggregate.Groups
{
    public class Group : Entity<GroupId>
    {
        private readonly List<Member> _students = new List<Member>();

        protected Group()
        {
        }

        internal Group(Number number, Sign sign, School school)
            : base(GroupId.New())
        {
            Number = number ?? throw new ArgumentNullException(nameof(number));
            Sign = sign ?? throw new ArgumentNullException(nameof(sign));
            School = school ?? throw new ArgumentNullException(nameof(school));
            IsArchived = false;
        }

        public Number Number { get; private set; }
        public Sign Sign { get; }
        public Code Code => new Code(Number, Sign);
        public bool IsArchived { get; private set; }
        public virtual Member FormTutor { get; private set; }
        public virtual Member Treasurer { get; private set; }
        public virtual School School { get; }
        public virtual IReadOnlyList<Member> Students => _students.AsReadOnly();

        internal Result<bool, Error> AssignFormTutor(MemberId memberId)
        {
            if (IsArchived)
                throw new InvalidOperationException(nameof(Group) + ":" + nameof(AssignFormTutor));

            var candidate = School.Members.Single(m => m.Id == memberId
                                                       && !m.IsArchived && m.Role == Role.Teacher);

            var groupOrNone = School.GroupOfFormTutor(candidate);

            var validation = Result.Combine(
                Result.FailureIf(Maybe<Member>.From(FormTutor).HasValue, true,
                    new Error($"Group '{Code}'(Id: '{Id}') already has a form tutor!")),
                Result.FailureIf(groupOrNone.HasValue, true,
                    new Error($"'{candidate.Email}'(Id: '{candidate.Id}') is already form tutor " +
                              $"of group '{groupOrNone.Value.Code}'!")));

            if (validation.IsFailure)
                return validation;

            FormTutor = candidate;

            return Result.Success<bool, Error>(true);
        }

        internal Result<bool, Error> AssignStudents(IEnumerable<MemberId> studentIds)
        {
            if (studentIds is null || !studentIds.Any())
                throw new ArgumentException(nameof(studentIds));

            if (IsArchived)
                throw new InvalidOperationException(nameof(Group) + ":" + nameof(AssignStudents));

            var studentsToAdd = School.Members.TakeWhile(
                m => studentIds.Contains(m.Id) && !m.IsArchived && m.Role == Role.Student);

            if (studentIds.Distinct().Count() != studentsToAdd.Count())
                throw new ArgumentException(nameof(studentIds));

            var validationResult = HaveSpaceFor(studentsToAdd);
            foreach (var student in studentsToAdd)
            {
                Maybe<Group> groupOrNone = student.Group;
                validationResult = Result.Combine(validationResult,
                    Result.FailureIf(groupOrNone.HasValue, true,
                        new Error($"'{student.Email}'(Id: '{student.Id}') " +
                                  $"is already member of group '{student.Group.Code}'!")));
            }

            if (validationResult.IsSuccess)
                _students.AddRange(studentsToAdd);

            return validationResult;
        }

        internal Result<bool, Error> CanGraduate()
        {
            if (IsArchived)
                throw new InvalidOperationException(nameof(Group) + ":" + nameof(CanGraduate));

            var validation = Result.Success<bool, Error>(true);
            if (Number == School.YearsOfEducation)
                foreach (var student in _students)
                    validation = Result.Combine(validation, student.CanBeArchived());

            return validation;
        }

        internal void Delete()
        {
            if (!IsArchived)
            {
                Maybe<Member> treasurerOrNone = Treasurer;
                if (treasurerOrNone.HasValue)
                    School.DivestTreasurerFromGroup(Id);

                Maybe<Member> formTutorOrNone = FormTutor;
                if (formTutorOrNone.HasValue)
                    School.DivestFormTutorFromGroup(Id);
            }
            else
            {
                Treasurer = null;
                FormTutor = null;
            }

            _students.Clear();
        }

        internal void DisenrollStudent(MemberId studentId)
        {
            var student = _students.Single(s => s.Id == studentId);

            if (Treasurer == student)
            {
                if (IsArchived)
                    DivestTreasurer();
                else
                    School.DivestTreasurerFromGroup(Id);
            }

            _students.Remove(student);
        }

        internal MemberId DivestFormTutor()
        {
            if (FormTutor == null)
                throw new InvalidOperationException(nameof(Group) + ":" + nameof(DivestFormTutor));

            var id = FormTutor.Id;

            FormTutor = null;

            return id;
        }

        internal MemberId DivestTreasurer()
        {
            if (Treasurer == null)
                throw new InvalidOperationException(nameof(Group) + ":" + nameof(DivestTreasurer));

            var id = Treasurer.Id;

            Treasurer = null;

            return id;
        }

        internal void Graduate()
        {
            if (IsArchived)
                throw new InvalidOperationException(nameof(Group) + ":" + nameof(Graduate));

            var studentIds = new List<MemberId>();
            if (Number == School.YearsOfEducation)
            {
                foreach (var student in Students)
                {
                    if (student.Archive().IsFailure)
                        throw new InvalidOperationException(nameof(Group) + ":" + nameof(Graduate));

                    studentIds.Add(student.Id);
                }

                IsArchived = true;

                return;
            }

            Number = Number.Create(Number + 1).Value;
        }

        internal Result<bool, Error> HaveSpaceFor(IEnumerable<Member> members)
        {
            if (members is null || !members.Any() || !members.All(m => m.School == School))
                throw new ArgumentException(nameof(members));

            return HaveSpaceFor(members.Count());
        }

        internal Result<bool, Error> HaveSpaceFor(int count)
        {
            Maybe<GroupMembersLimit> limit = School.GroupMembersLimit;
            if (limit.HasValue && _students.Count + count > limit.Value)
            {
                var diff = School.GroupMembersLimit - Students.Count;
                var exceededBy = _students.Count + count - limit.Value;
                var diffMessage = diff > 0
                    ? $"Maximally '{diff.Value}' members can be added!"
                    : "Cannot add any more members!";
                var message = $"Member limit for group '{Code}' (Id: '{Id.Value}') exceeded by '{exceededBy}'!" +
                              diffMessage;

                return new Error(message);
            }

            return Result.Success<bool, Error>(true);
        }


        internal Result<bool, Error> PromoteTreasurer(MemberId studentId)
        {
            if (IsArchived)
                throw new InvalidOperationException(nameof(Group) + ":" + nameof(PromoteTreasurer));

            var student = _students.Single(m => m.Id == studentId);

            Maybe<Member> formTutorOrNone = Treasurer;

            if (formTutorOrNone.HasValue)
                return new Error($"Group '{Code}'(Id: '{Id}') already has a treasurer!");

            Treasurer = student;

            return Result.Success<bool, Error>(true);
        }
    }
}