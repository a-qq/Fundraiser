using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using SchoolManagement.Application.Common.Interfaces;
using SharedKernel.Infrastructure.Interfaces;
using SharedKernel.Infrastructure.Options;

namespace SchoolManagement.Infrastructure.Services
{
    internal sealed class ManagementMailManager : IManagementMailManager
    {
        private readonly IMailManager _mailManager;
        private readonly UrlsOptions _urls;
        private readonly string _welcomeTemplate;

        public ManagementMailManager(
            IOptions<UrlsOptions> urls,
            IWebHostEnvironment environment,
            IMailManager mailManager,
            IMemoryCache cache)
        {
            _welcomeTemplate = GetWelcomeEmailTemplate(environment, cache);
            _urls = urls.Value;
            _mailManager = mailManager;
        }

        public async Task SendRegistrationEmailAsync(string firstName, string email, string securityCode)
        {
            var subject = $"{firstName}, welcome in your school's managment system!";
            var url = $"{_urls.Idp}Registration/Register/?SecurityCode={securityCode.Replace("+", "%2B")}";
            var body = PopulateWelcomeTemplate(subject, firstName, email, url);
            await _mailManager.SendMailAsHtmlAsync(email, subject, body);
        }

        private string PopulateWelcomeTemplate(string subject, string firstName, string email, string url)
        {
            var body = _welcomeTemplate.Replace("{FirstName}", firstName);
            body = body.Replace("{Url}", url);
            body = body.Replace("{Email}", email);
            body = body.Replace("{Subject}", subject);
            body = body.Replace("{base}",
                Environment.GetEnvironmentVariable("ASPNETCORE_URLS").Split(';')[1] + "/templates/");
            return body;
        }

        private static string GetWelcomeEmailTemplate(IWebHostEnvironment environment, IMemoryCache cache)
        {
            if (!cache.TryGetValue("WelcomeEmailTemplate", out string welcomeEmailTemplate))
            {
                var templatePath = Path.Combine(environment.WebRootPath, @"templates\WelcomeEmailTemplate.html");
                using (var reader = new StreamReader(templatePath))
                {
                    welcomeEmailTemplate = reader.ReadToEnd();
                }

                cache.Set("WelcomeEmailTemplate", welcomeEmailTemplate);
            }

            return welcomeEmailTemplate;
        }
    }
}