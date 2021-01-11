using Fundraiser.SharedKernel.Settings;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Fundraiser.SharedKernel.Managers
{
    public class MailManager : IMailManager
    {
        private readonly MailSettings _mailSettings;
        private SmtpClient _client;

        public MailManager(IOptions<MailSettings> mailOptions)
        {
            _mailSettings = mailOptions.Value;
            InitializeSmtpClient();
        }

        public async Task SendMailAsync(string receiver, string subject, string message)
        {
            using (var emailMessage = new MailMessage())
            {
                emailMessage.To.Add(new MailAddress(receiver));
                emailMessage.From = new MailAddress(_mailSettings.Email);
                emailMessage.Subject = subject;
                emailMessage.Body = message;
                _client.Send(emailMessage);
            }
            await Task.CompletedTask;
        }

        private void InitializeSmtpClient()
        {
            _client = new SmtpClient
            {
                Credentials = GetCredentials(),
                Host = _mailSettings.Host,
                Port = _mailSettings.Port,
                EnableSsl = true
            };
        }

        private NetworkCredential GetCredentials()
        {
            return new NetworkCredential
            {
                UserName = _mailSettings.Email,
                Password = _mailSettings.Password
            };
        }
    }
}
