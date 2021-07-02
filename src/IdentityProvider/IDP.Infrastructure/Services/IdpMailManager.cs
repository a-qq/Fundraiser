using Ardalis.GuardClauses;
using IDP.Application.Common.Interfaces;
using IDP.Domain.UserAggregate.ValueObjects;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using SharedKernel.Domain.ValueObjects;
using SharedKernel.Infrastructure.Abstractions.Common;
using SharedKernel.Infrastructure.Options;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IDP.Infrastructure.Services
{
    internal sealed class IdpMailManager : IIdpMailManager
    {
        private readonly IMailManager _mailManager;
        private readonly UrlsOptions _urls;
        private readonly IWebHostEnvironment _environment;
        private readonly IMemoryCache _cache;

        public IdpMailManager(
            UrlsOptions urlsOptions,
            IWebHostEnvironment webHostEnvironment,
            IMailManager mailManager,
            IMemoryCache memoryCache)
        {
            _cache = Guard.Against.Null(memoryCache, nameof(memoryCache));
            _environment = Guard.Against.Null(webHostEnvironment, nameof(webHostEnvironment));
            _urls = Guard.Against.Null(urlsOptions, nameof(urlsOptions));
            _mailManager = Guard.Against.Null(mailManager, nameof(mailManager));
        }

        //        await _mailManager.SendMailAsHtmlAsync(domainEvent.Email, "Your new reset password link from your school's management system!",
        //$"<hmtl><body><p>Please click the link below to reset your password!<br/>" +
        //$"</p><p><a href='{url}' > Reset password</a></p>" +
        //$"<p>This link will be valid until {domainEvent.SecurityCode.ExpirationDate.Date.Value:MM/dd/yyyy HH:mm}.</p>" +
        //$"<p>If you haven't requested password reset please ignore this message!</p></body></html>");


        public async Task SendResetPasswordEmail(Email email, SecurityCode securityCode)
        {
            string emailString = Guard.Against.Null(email, nameof(email));
            string securityCodeString = Guard.Against.Null(securityCode, nameof(securityCode));

            var subject = "Password reset for your account has been requested!";
            var url = $"{_urls.Idp}PasswordReset/ResetPassword/?securityCode={securityCodeString.Replace("+", "%2B")}";
            var body = PopulateResetPasswordTemplate(subject, emailString, url, securityCode.ExpirationDate);

            await _mailManager.SendMailAsHtmlAsync(email, subject, body);
        }

        private string PopulateResetPasswordTemplate(string subject, string email, string url, DateTime? expirationDate)
        {
            var body = GetResetPasswordTemplate(_environment, _cache);;
            body = body.Replace("{Url}", url);
            body = body.Replace("{Email}", email);
            body = body.Replace("{Subject}", subject);
            body = body.Replace("{base}",
                GetHostUrl() + "/templates/");
            body = body.Replace("{expirationDate}",
                 expirationDate?.ToString("MM/dd/yyyy HH:mm") ?? "consumed");

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

        public async Task SendRegistrationEmailAsync(Email email, SecurityCode securityCode, string givenName)
        {
            string emailString = Guard.Against.Null(email, nameof(email));
            string securityCodeString = Guard.Against.Null(securityCode, nameof(securityCode));

            var subject = $"{givenName}, welcome in your school's management system!";
            var url = $"{_urls.Idp}Registration/Register/?SecurityCode={securityCodeString.Replace("+", "%2B")}";
            var body = PopulateWelcomeTemplate(subject, givenName ?? "User", emailString, url);
            await _mailManager.SendMailAsHtmlAsync(emailString, subject, body);
        }

        private string PopulateWelcomeTemplate(string subject, string givenName, string email, string url)
        {
            var body = GetWelcomeEmailTemplate();
            body = body.Replace("{FirstName}", givenName);
            body = body.Replace("{Url}", url);
            body = body.Replace("{Email}", email);
            body = body.Replace("{Subject}", subject);
            body = body.Replace("{base}",
                GetHostUrl() + "/templates/");
            return body;
        }

        private string GetWelcomeEmailTemplate()
        {
            if (!_cache.TryGetValue("WelcomeEmailTemplate", out string welcomeEmailTemplate))
            {
                var templatePath = Path.Combine(_environment.WebRootPath, @"templates\WelcomeEmailTemplate.html");
                using (var reader = new StreamReader(templatePath))
                {
                    welcomeEmailTemplate = reader.ReadToEnd();
                }

                _cache.Set("WelcomeEmailTemplate", welcomeEmailTemplate);
            }

            return welcomeEmailTemplate;
        }

        private static string GetHostUrl()
            => Environment.GetEnvironmentVariable("ASPNETCORE_URLS").Split(';').First(url => url.StartsWith("https"));
    }
}