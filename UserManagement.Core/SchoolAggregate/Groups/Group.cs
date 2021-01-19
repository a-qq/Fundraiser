using CSharpFunctionalExtensions;
using SchoolManagement.Core.SchoolAggregate.Schools;
using SchoolManagement.Core.SchoolAggregate.Members;
using System;
using System.Collections.Generic;

namespace SchoolManagement.Core.SchoolAggregate.Groups
{
    public class Group : Entity
    {
        private readonly List<Member> _members = new List<Member>();

        public Number Number { get; private set; }
        public Sign Sign { get; private set; }
        public string Code => Number + Sign;
        public virtual Member FormTutor { get; private set; }
        public virtual School School { get; private set; }
        public virtual IReadOnlyList<Member> Members => _members.AsReadOnly();

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
