using CSharpFunctionalExtensions;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Domain.Errors;
using SharedKernel.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SchoolManagement.Domain.SchoolAggregate.Groups
{
    public class Group : Entity<GroupId>
    {
        private readonly List<Member> _students = new List<Member>();

        public Number Number { get; private set; }
        public Sign Sign { get; private set; }
        public Code Code => new Code(Number, Sign);
        public bool IsArchived { get; private set; }
        public virtual Member FormTutor { get; private set; }
        public virtual Member Treasurer { get; private set; }
        public virtual School School { get; private set; }
        public virtual IReadOnlyList<Member> Students => _students.AsReadOnly();

        protected Group() { }

        internal Group(Number number, Sign sign, School school)
            : base(GroupId.New())
        {
            Number = number ?? throw new ArgumentNullException(nameof(number));
            Sign = sign ?? throw new ArgumentNullException(nameof(sign));
            School = school ?? throw new ArgumentNullException(nameof(school));
            IsArchived = false;
        }

        internal Result<bool, Error> AssignFormTutor(MemberId memberId)
        {
            if (this.IsArchived)
                throw new InvalidOperationException(nameof(Group) + ":" + nameof(AssignFormTutor));

            Member candidate = this.School.Members.Single(m => m.Id == memberId
                && !m.IsArchived && m.Role == Role.Teacher);

            var groupOrNone = this.School.GroupOfFormTutor(candidate);

            var validation = Result.Combine(
                Result.FailureIf(Maybe<Member>.From(this.FormTutor).HasValue, true,
                    new Error($"Group '{this.Code}'(Id: '{this.Id}') already has a form tutor!")),
                Result.FailureIf(groupOrNone.HasValue, true,
                    new Error($"'{candidate.Email}'(Id: '{candidate.Id}') is already form tutor " +
                        $"of group '{groupOrNone.Value.Code}'!")));

            if (validation.IsFailure)
                return validation;

            this.FormTutor = candidate;

            return Result.Success<bool, Error>(true);
        }

        internal Result<bool, Error> AssignStudents(IEnumerable<MemberId> studentIds)
        {
            if (studentIds is null || !studentIds.Any())
                throw new ArgumentException(nameof(studentIds));

            if (this.IsArchived)
                throw new InvalidOperationException(nameof(Group) + ":" + nameof(AssignStudents));

            IEnumerable<Member> studentsToAdd = this.School.Members.TakeWhile(
                m => studentIds.Contains(m.Id) && !m.IsArchived && m.Role == Role.Student);

            if (studentIds.Distinct().Count() != studentsToAdd.Count())
                throw new ArgumentException(nameof(studentIds));

            Result<bool, Error> validationResult = HaveSpaceFor(studentsToAdd);
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
            if (this.IsArchived)
                throw new InvalidOperationException(nameof(Group) + ":" + nameof(CanGraduate));

            var validation = Result.Success<bool, Error>(true);
            if (this.Number == this.School.YearsOfEducation)
            {
                foreach (var student in this._students)
                    validation = Result.Combine(validation, student.CanBeArchived());
            }

            return validation;
        }

        internal void Delete()
        {
            if (!IsArchived)
            {
                Maybe<Member> treasurerOrNone = this.Treasurer;
                if (treasurerOrNone.HasValue)
                    this.School.DivestTreasurerFromGroup(this.Id);

                Maybe<Member> formTutorOrNone = this.FormTutor;
                if (formTutorOrNone.HasValue)
                    this.School.DivestFormTutorFromGroup(this.Id);
            }
            else
            {
                this.Treasurer = null;
                this.FormTutor = null;
            }

            this._students.Clear();
        }

        internal void DisenrollStudent(MemberId studentId)
        {
            Member student = this._students.Single(s => s.Id == studentId);

            if (this.Treasurer == student)
            {
                if (this.IsArchived)
                    this.DivestTreasurer();
                else
                    this.School.DivestTreasurerFromGroup(this.Id);
            }
            this._students.Remove(student);
        }

        internal MemberId DivestFormTutor()
        {
            if (this.FormTutor == null)
                throw new InvalidOperationException(nameof(Group) + ":" + nameof(DivestFormTutor));

            MemberId id = this.FormTutor.Id;

            this.FormTutor = null;

            return id;
        }

        internal MemberId DivestTreasurer()
        {
            if (this.Treasurer == null)
                throw new InvalidOperationException(nameof(Group) + ":" + nameof(DivestTreasurer));

            MemberId id = this.Treasurer.Id;

            this.Treasurer = null;

            return id;
        }

        internal void Graduate()
        {
            if (this.IsArchived)
                throw new InvalidOperationException(nameof(Group) + ":" + nameof(Graduate));

            List<MemberId> studentIds = new List<MemberId>();
            if (this.Number == this.School.YearsOfEducation)
            {
                foreach (var student in this.Students)
                {
                    if (student.Archive().IsFailure)
                        throw new InvalidOperationException(nameof(Group) + ":" + nameof(Graduate));

                    studentIds.Add(student.Id);
                }

                this.IsArchived = true;

                return;
            }

            this.Number = Number.Create(this.Number + 1).Value;
        }

        internal Result<bool, Error> HaveSpaceFor(IEnumerable<Member> members)
        {
            if (members is null || !members.Any() || !members.All(m => m.School == this.School))
                throw new ArgumentException(nameof(members));

            return HaveSpaceFor(members.Count());
        }

        internal Result<bool, Error> HaveSpaceFor(int count)
        {
            Maybe<GroupMembersLimit> limit = this.School.GroupMembersLimit;
            if (limit.HasValue && (this._students.Count + count) > limit.Value)
            {
                var diff = this.School.GroupMembersLimit - this.Students.Count;
                var exceededBy = this._students.Count + count - limit.Value;
                string diffMessage = diff > 0 ? $"Maximally '{diff.Value}' members can be added!" : "Cannot add any more members!";
                string message = $"Member limit for group '{this.Code}' (Id: '{this.Id.Value}') exceeded by '{exceededBy}'!" + diffMessage;

                return new Error(message);
            }

            return Result.Success<bool, Error>(true);
        }


        internal Result<bool, Error> PromoteTreasurer(MemberId studentId)
        {
            if (this.IsArchived)
                throw new InvalidOperationException(nameof(Group) + ":" + nameof(PromoteTreasurer));

            Member student = this._students.Single(m => m.Id == studentId);

            Maybe<Member> formTutorOrNone = this.Treasurer;

            if (formTutorOrNone.HasValue)
                return new Error($"Group '{this.Code}'(Id: '{this.Id}') already has a treasurer!");

            this.Treasurer = student;

            return Result.Success<bool, Error>(true);
        }
    }
}
