using System.Threading.Tasks;

namespace Fundraiser.SharedKernel.Managers
{
    public interface IMailManager
    {
        Task SendMailAsync(string receiver, string subject, string message);
    }
}
