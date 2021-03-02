using System.Threading.Tasks;

namespace SchoolManagement.Application.Common.Interfaces
{
    public interface IManagementMailManager
    {
        Task SendRegistrationEmailAsync(string firstName, string email, string securityCode);
    }
}