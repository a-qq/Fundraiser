using System.Threading.Tasks;

namespace SharedKernel.Infrastructure.Abstractions.Common
{
    public interface IMailManager
    {
        Task SendMailAsHtmlAsync(string receiver, string subject, string message);
    }
}