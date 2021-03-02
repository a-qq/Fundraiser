namespace IDP.Application.DTOs
{
    public class UserDTO
    {
        public UserDTO(string subject, string email)
        {
            Subject = subject;
            Email = email;
        }

        public string Subject { get; set; }
        public string Email { get; set; }
    }
}
