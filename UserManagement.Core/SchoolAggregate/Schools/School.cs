using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.ResultErrors;
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
        public virtual IReadOnlyList<User> Members => _members.ToList();
        public virtual IReadOnlyList<Group> Groups => _groups.ToList();
        protected School()
        {
        }
        internal School(Name name)
            : base(Guid.NewGuid())
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        internal Result<bool, RequestError> Enroll(User candidate)
        {
            if (candidate == null)
                throw new ArgumentNullException(nameof(candidate));

            var validationResult = CanBeEnrolled(candidate);
            if (validationResult.IsFailure)
                return validationResult;

            _members.Add(candidate);

            AddDomainEvent(new UserEnrolledEvent(candidate.Id, candidate.FirstName,
                candidate.LastName, candidate.Email, candidate.Role, candidate.Gender, Id));

            return Result.Success<bool, RequestError>(true);
        }

        private Result<bool, RequestError> CanBeEnrolled(User candidate)
        {
            if (candidate.Role > Role.Headmaster)
                return Result.Failure<bool, RequestError>(SharedErrors.General.BusinessRuleViolation("Role out of scope for school members!"));

            if (candidate.Role == Role.Headmaster && _members.Any(m => m.Role == Role.Headmaster))
                return Result.Failure<bool, RequestError>(SharedErrors.General.BusinessRuleViolation("School already have a headmaster, only one headmaster per school is allowed!"));

            return Result.Success<bool, RequestError>(true);
        }
    }
}
