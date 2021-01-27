using Fundraiser.SharedKernel.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Fundraiser.SharedKernel.Managers
{
    public class MailManager : IMailManager
    {
        private readonly MailSettings _mailSettings;
        private readonly SmtpClient _client;
        private readonly string _welcomeTemplate;
        public MailManager(
            IOptions<MailSettings> mailOptions,
            IWebHostEnvironment environment)
        {
            _mailSettings = mailOptions.Value;
            _welcomeTemplate = GetTemplates(environment);
            _client = InitializeSmtpClient();
        }

        public async Task SendMailAsync(string receiver, string subject, string message)
        {
            var mimeType = new ContentType("text/html");
            using (var emailMessage = new MailMessage())
            using (AlternateView alternate = AlternateView.CreateAlternateViewFromString(message, mimeType))
            {
                emailMessage.To.Add(new MailAddress(receiver));
                emailMessage.From = new MailAddress(_mailSettings.Email);
                emailMessage.Subject = subject;
                emailMessage.AlternateViews.Add(alternate);
                _client.Send(emailMessage);
            }
            await Task.CompletedTask;
        }

        private string GetTemplates(IWebHostEnvironment environment)
        {
            //TODO: Cache the template(s)
            var templatePath = Path.Combine(environment.WebRootPath, @"templates\WelcomeEmailTemplate.html");
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(templatePath))
            {
                body = reader.ReadToEnd();
            }
            return body;
        }

        public string PopulateWelcomeTemplate(string subject, string firstName, string email, string url)
        {
            string body = _welcomeTemplate.Replace("{FirstName}", firstName);
            body = body.Replace("{Url}", url);
            body = body.Replace("{Email}", email);
            body = body.Replace("{Subject}", subject);
            body = body.Replace("{base}", Environment.GetEnvironmentVariable("ASPNETCORE_URLS").Split(';')[1] + "/templates/");
            return body;
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
    }
}
