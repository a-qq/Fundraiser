using System.Threading.Tasks;

namespace Fundraiser.SharedKernel.Managers
{
    public interface IMailManager
    {
        public string PopulateWelcomeTemplate(string subject, string firstName, string email, string url);
        Task SendMailAsync(string receiver, string subject, string message);
    }
}
