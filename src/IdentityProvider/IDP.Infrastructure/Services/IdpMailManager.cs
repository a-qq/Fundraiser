using System;
using System.IO;
using System.Threading.Tasks;
using IDP.Application.Common.Interfaces;
using IDP.Domain.UserAggregate.ValueObjects;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using SharedKernel.Domain.ValueObjects;
using SharedKernel.Infrastructure.Interfaces;
using SharedKernel.Infrastructure.Options;

namespace IDP.Infrastructure.Services
{
    internal sealed class IdpMailManager : IIdpMailManager
    {
        private readonly IMailManager _mailManager;
        private readonly string _resetPasswordTemplate;
        private readonly UrlsOptions _urls;

        public IdpMailManager(
            IOptions<UrlsOptions> urls,
            IWebHostEnvironment environment,
            IMailManager mailManager,
            IMemoryCache memoryCache)
        {
            _resetPasswordTemplate = GetResetPasswordTemplate(environment, memoryCache);
            _urls = urls.Value;
            _mailManager = mailManager;
        }

        //        await _mailManager.SendMailAsHtmlAsync(domainEvent.Email, "Your new reset password link from your school's management system!",
        //$"<hmtl><body><p>Please click the link below to reset your password!<br/>" +
        //$"</p><p><a href='{url}' > Reset password</a></p>" +
        //$"<p>This link will be valid until {domainEvent.SecurityCode.ExpirationDate.Date.Value:MM/dd/yyyy HH:mm}.</p>" +
        //$"<p>If you haven't requested password reset please ignore this message!</p></body></html>");


        public async Task SendResetPasswordEmail(Email email, SecurityCode securityCode)
        {
            if (email is null)
                throw new ArgumentNullException(nameof(email));

            if (securityCode is null)
                throw new ArgumentNullException(nameof(securityCode));

            var subject = "Password reset for your account has been requested!";
            var url = $"{_urls.Idp}PasswordReset/ResetPassword/?securityCode={securityCode.Value.Replace("+", "%2B")}";
            var body = PopulateResetPasswordTemplate(subject, email, url, securityCode.ExpirationDate.Date.Value);

            await _mailManager.SendMailAsHtmlAsync(email, subject, body);
        }

        private string PopulateResetPasswordTemplate(string subject, string email, string url, DateTime? expirationDate)
        {
            var body = _resetPasswordTemplate.Replace("{Url}", url);
            body = body.Replace("{Email}", email);
            body = body.Replace("{Subject}", subject);
            body = body.Replace("{base}",
                Environment.GetEnvironmentVariable("ASPNETCORE_URLS").Split(';')[1] + "/templates/");
            body = body.Replace("{expirationDate}",
                expirationDate.HasValue ? expirationDate?.ToString("MM/dd/yyyy HH:mm") : "consumed");

            return body;
        }

        private static string GetResetPasswordTemplate(IWebHostEnvironment environment, IMemoryCache cache)
        {
            if (!cache.TryGetValue("ResetPasswordTemplate", out string resetPasswordTemplate))
            {
                var templatePath = Path.Combine(environment.WebRootPath, @"templates\ResetPasswordTemplate.html");
                using (var reader = new StreamReader(templatePath))
                {
                    resetPasswordTemplate = reader.ReadToEnd();
                }

                cache.Set("ResetPasswordTemplate", resetPasswordTemplate);
            }

            return resetPasswordTemplate;
        }
    }
}