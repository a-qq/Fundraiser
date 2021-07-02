namespace IDP.Application.DTOs
{
    public class UserDto
    {
        public UserDto(string subject, string email)
        {
            Subject = subject;
            Email = email;
        }

        public string Subject { get; set; }
        public string Email { get; set; }
    }
}