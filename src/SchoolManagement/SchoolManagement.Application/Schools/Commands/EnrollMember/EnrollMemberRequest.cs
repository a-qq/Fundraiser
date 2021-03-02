namespace SchoolManagement.Application.Schools.Commands.EnrollMember
{
    public sealed class EnrollMemberRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Gender { get; set; }
    }
}