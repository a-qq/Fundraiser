using System;

namespace SchoolManagement.Data.Schools
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Gender { get; set; }
    }
}
