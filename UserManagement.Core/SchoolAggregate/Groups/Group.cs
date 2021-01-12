using CSharpFunctionalExtensions;
using SchoolManagement.Core.SchoolAggregate.Schools;
using SchoolManagement.Core.SchoolAggregate.Users;
using System.Collections.Generic;
using System.Linq;

namespace SchoolManagement.Core.SchoolAggregate.Groups
{
    public class Group : Entity
    {
        private readonly List<User> _members = new List<User>();

        public byte CodeNumber { get; private set; }
        public char CodeLetter { get; private set; }
        public virtual User FormTutor { get; private set; }
        public virtual School School { get; private set; }
        public virtual IReadOnlyList<User> Members => _members.AsReadOnly();

        protected Group()
        {
        }
    }
}
