using System.Threading.Tasks;

namespace SharedKernel.Infrastructure.Interfaces
{
    public interface IMailManager
    {
        Task SendMailAsHtmlAsync(string receiver, string subject, string message);
    }
}
