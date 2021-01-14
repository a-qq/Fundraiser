using CSharpFunctionalExtensions;
using SchoolManagement.Core.SchoolAggregate.Schools;
using SchoolManagement.Core.SchoolAggregate.Users;
using System;
using System.Collections.Generic;

namespace SchoolManagement.Core.SchoolAggregate.Groups
{
    public class Group : Entity
    {
        private readonly List<User> _members = new List<User>();

        public Number Number { get; private set; }
        public Sign Sign { get; private set; }
        public string Code => Number + Sign;
        public virtual User FormTutor { get; private set; }
        public virtual School School { get; private set; }
        public virtual IReadOnlyList<User> Members => _members.AsReadOnly();

        protected Group()
        {
        }

        internal Group(Number number, Sign sign, School school)
        {
            Number = number ?? throw new ArgumentNullException();
            Sign = sign ?? throw new ArgumentNullException();
            School = school ?? throw new ArgumentNullException();
        }
    }
}
