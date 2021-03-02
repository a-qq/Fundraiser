using System;

namespace SchoolManagement.Application.Schools
{
    public sealed class MemberDTO
    {
        public Guid Id { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string Role { get; private set; }
        public string Gender { get; private set; }
    }
}
