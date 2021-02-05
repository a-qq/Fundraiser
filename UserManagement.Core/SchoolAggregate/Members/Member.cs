using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.Utils;
using SchoolManagement.Core.SchoolAggregate.Groups;
using SchoolManagement.Core.SchoolAggregate.Schools;
using System;

namespace SchoolManagement.Core.SchoolAggregate.Members
{
    public class Member : Entity<Guid>
    {
        public FirstName FirstName { get; }
        public LastName LastName { get; }
        public Role Role { get; private set; }
        public Email Email { get; }
        public Gender Gender { get; }
        public bool IsActive { get; }
        public bool IsArchived { get; private set; }
        public virtual School School { get; }
        public virtual Group Group { get; private set; }


        protected Member()
        {
        }

        internal Member(FirstName firstName, LastName lastName, Email email, Role role, Gender gender, School school)
            : base(Guid.NewGuid())
        {
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            Role = role ?? throw new ArgumentNullException(nameof(role));
            Gender = gender ?? throw new ArgumentNullException(nameof(gender));
            School = school ?? throw new ArgumentNullException(nameof(school));
            IsActive = false;
            IsArchived = false;
        }

        internal void Archive()
            => IsArchived = true;
    }
}
