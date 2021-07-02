using System;
using SharedKernel.Infrastructure.Abstractions.Common;
using SharedKernel.Infrastructure.Options;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;

namespace SharedKernel.Infrastructure.Concretes.Services
{
    public sealed class MailManager : IMailManager, IDisposable
    {
        private readonly SmtpClient _client;
        private readonly MailOptions _mailSettings;

        public MailManager(MailOptions mailOptions)
        {
            _mailSettings = mailOptions;
            _client = InitializeSmtpClient();
        }

        public async Task SendMailAsHtmlAsync(string receiver, string subject, string message)
        {
            var mimeType = new ContentType("text/html");
            using (var emailMessage = new MailMessage())
            using (var alternate = AlternateView.CreateAlternateViewFromString(message, mimeType))
            {
                emailMessage.To.Add(new MailAddress(receiver));
                emailMessage.From = new MailAddress(_mailSettings.Email);
                emailMessage.Subject = subject;
                emailMessage.AlternateViews.Add(alternate);
                await _client.SendMailAsync(emailMessage);
            }
        }

        private SmtpClient InitializeSmtpClient()
        {
            return new SmtpClient
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

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}