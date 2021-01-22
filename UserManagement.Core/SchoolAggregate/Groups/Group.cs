using CSharpFunctionalExtensions;
using SchoolManagement.Core.SchoolAggregate.Schools;
using SchoolManagement.Core.SchoolAggregate.Members;
using System;
using System.Collections.Generic;
using Fundraiser.SharedKernel.Utils;
using System.Linq;

namespace SchoolManagement.Core.SchoolAggregate.Groups
{
    public class Group : Entity
    {
        private readonly List<Member> _students = new List<Member>();

        public Number Number { get; private set; }
        public Sign Sign { get; private set; }
        public string Code => Number + Sign;
        public bool IsArchived { get; private set; }
        public virtual Member FormTutor { get; private set; }
        public virtual School School { get; private set; }
        public virtual IReadOnlyList<Member> Students => _students.AsReadOnly();
        
        protected Group()
        {
        }

        internal Group(Number number, Sign sign, School school)
        {
            Number = number ?? throw new ArgumentNullException();
            Sign = sign ?? throw new ArgumentNullException();
            School = school ?? throw new ArgumentNullException();
            IsArchived = false;
        }

        internal Result<bool, Error> AssignMembers(IEnumerable<Member> members)
        {
            if (members.Count() != members.Distinct().Count())
                throw new InvalidOperationException(nameof(AssignMembers));

            if (this.School.GroupMembersLimit != null && (this.Students.Count + members.Count() > this.School.GroupMembersLimit))
            {
                var diff = this.School.GroupMembersLimit - this.Students.Count;
                string diffMessage = diff > 0 ? $"Maximally'{diff}' members can be added" : "Cannot add any more members";
                string message = "Group's member limit exceeded! " + diffMessage + $" to group '{this.Code}!";
                return Result.Failure<bool, Error>(new Error(message));
            }
                

            Result<bool, Error> validationResult = Result.Success<bool, Error>(true);
            foreach (var member in members)
            {
                if (member.School != this.School || member.IsArchived)
                    throw new InvalidOperationException(nameof(AssignMembers));

                validationResult = Result.Combine(validationResult,
                    Result.FailureIf(member.Group != null, true, new Error($"'{member.Email.Value}'(Id: '{member.Id}') is already member of group '{this.Code}'!")),
                    Result.FailureIf(member.Role != Role.Student, true, new Error($"'{member.Email.Value}'(Id: '{member.Id}') is not a {Role.Student}!'")));
            }

            if (validationResult.IsFailure)
                return validationResult;

            _students.AddRange(members);

            return Result.Success<bool, Error>(true);
        }

        internal Result AssignFormTutor(Member member)
        {
            if (this.IsArchived)
                throw new InvalidOperationException(nameof(AssignFormTutor));

            Result validation = this.School.CanBeFormTutor(member);
            if (validation.IsFailure)
                return validation;

            this.FormTutor = member;

            return Result.Success();
        }

        internal Result DivestFormTutor()
        {
            if (this.IsArchived)
                throw new InvalidOperationException(nameof(AssignFormTutor));

            if (this.FormTutor == null)
                return Result.Failure($"Group '{Code}'(Id: '{Id}') doest not have a form tutor!");

            return Result.Success();
        }
    }
}
