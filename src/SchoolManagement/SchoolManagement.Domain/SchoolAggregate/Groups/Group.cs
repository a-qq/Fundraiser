using CSharpFunctionalExtensions;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Domain.Constants;
using SharedKernel.Domain.Errors;
using System;
using System.Collections.Generic;
using System.Linq;

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

            var candidate = School.Members.Single(
                m => m.Id == memberId && !m.IsArchived && m.Role == Role.Teacher);

            var groupOrNone = School.CurrentGroupOfFormTutor(candidate);

            var validation = Result.Success<bool, Error>(true);

            if (!(FormTutor is null))
                validation = new Error($"Group '{Code}'(Id: '{Id}') already has a form tutor!");

            if (groupOrNone.HasValue)
                validation = Result.Combine(validation, new Error($"'{candidate.Email}'(Id: '{candidate.Id}')" +
                                    $" is already form tutor of group '{groupOrNone.Value.Code}'!"));

            if (validation.IsFailure)
                return validation;

            FormTutor = candidate;

            return Result.Success<bool, Error>(true);
        }

        internal Result<Member, Error> AssignStudent(MemberId memberId)
        {
            var student = School.Members.Single(
                m => m.Id == memberId && !m.IsArchived && m.Role == Role.Student);

            if (!(student.Group is null))
            {
                return new Error($"'{student.Email}' (Id: '{student.Id}') is " +
                                 $"already member of group '{student.Group.Code}'!");
            }

            if (!(School.GroupMembersLimit is null) && _students.Count >= School.GroupMembersLimit)
            {
                return new Error($"Group '{Code}' (Id: '{Id}') is full (max " +
                                 $"student count: '{School.GroupMembersLimit}')!");
            }

            _students.Add(student);
            student.SetGroup(this);

            return Result.Success<Member, Error>(student);
        }

        internal void Delete()
        {
            if (!IsArchived)
            {
                if (!(FormTutor is null))
                    School.DivestFormTutorFromGroup(Id);

                foreach (var student in Students.ToList())
                    School.DisenrollStudentFromGroup(Id, student.Id);

                return;
            }

            this.FormTutor = null;

            foreach (var student in Students.ToList())
                DisenrollStudent(student.Id);
        }

        internal (Member, string) DisenrollStudent(MemberId studentId)
        {
            var student = _students.Single(s => s.Id == studentId);
            var isTreasurer = Treasurer == student;
            if (isTreasurer)
                DivestTreasurer();
            
            _students.Remove(student);
            student.RemoveFromGroup();

            return (student, isTreasurer ? GroupRoles.Treasurer : null);
        }

        internal Member DivestFormTutor()
        {
            if (FormTutor == null)
                throw new InvalidOperationException(nameof(Group) + ":" + nameof(DivestFormTutor));

            var formTutor = FormTutor;

            FormTutor = null;

            return formTutor;
        }

        internal Member DivestTreasurer()
        {
            if (Treasurer == null)
                throw new InvalidOperationException(nameof(Group) + ":" + nameof(DivestTreasurer));

            var treasurer = Treasurer;

            Treasurer = null;

            return treasurer;
        }

        internal Result<bool, Error> Graduate()
        {
            if (IsArchived)
                throw new InvalidOperationException("Attempt to graduate archived group!");

            if (Number >= School.YearsOfEducation)
            {
                //order matters, treasurer role wil be kept, but student won't have perms

                IsArchived = true;

                var result = Result.Success<bool, Error>(true);
                foreach (var student in Students)
                {
                    var archivization = School.ArchiveMember(student.Id);
                    result = Result.Combine(result, archivization);
                }

                return result;
            }

            Number = Number.Create(Number + 1).Value;
            return Result.Success<bool, Error>(true);
        }

        public Result<bool, Error> HaveSpaceFor(int count)
        {
            Maybe<GroupMembersLimit> limit = School.GroupMembersLimit;

            if (!limit.HasValue || !(_students.Count + count > limit.Value))
                return Result.Success<bool, Error>(true);

            var diff = School.GroupMembersLimit - Students.Count;

            var exceededBy = _students.Count + count - limit.Value;

            var diffMessage = diff > 0
                ? $"Maximally '{diff.Value}' members can be added!"
                : "Cannot add any more members!";

            var message = $"Member limit for group '{Code}' (Id: '{Id.Value}') " +
                          $"exceeded by '{exceededBy}'!" + diffMessage;

            return new Error(message);
        }


        internal Result<bool, Error> PromoteTreasurer(MemberId studentId)
        {
            if (IsArchived)
                throw new InvalidOperationException(nameof(Group) + ":" + nameof(PromoteTreasurer));

            var student = _students.Single(m => m.Id == studentId);

            if (!(Treasurer is null))
                return new Error($"Group '{Code}' (Id: '{Id}') already has a treasurer!");

            Treasurer = student;

            return Result.Success<bool, Error>(true);
        }
    }
}