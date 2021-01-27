using System;

namespace SchoolManagement.Data.Schools.EnrollMembersFromCsv
{
    public class MemberCreatedDTO
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Gender { get; set; }
        public long? GroupId { get; set; }
    }
}
