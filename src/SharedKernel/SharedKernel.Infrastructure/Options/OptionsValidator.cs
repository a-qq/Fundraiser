﻿using System;
using System.Linq;
using Microsoft.Extensions.Options;
using SharedKernel.Domain.ValueObjects;

namespace SharedKernel.Infrastructure.Options
{
    public static class SharedOptionsValidator
    {
        public static void ValidateMailOptions(IOptions<MailOptions> mailOptions)
        {
            var mailConfig = mailOptions.Value;
            var emailValidation = Email.Validate(mailConfig.Email);
            if (emailValidation.IsFailure)
                throw new ApplicationException(string.Join(" \n", emailValidation.Error.Errors));

            if (string.IsNullOrWhiteSpace(mailConfig.Host))
                throw new ApplicationException($"{mailConfig.Host} in {MailOptions.MailSettings} is not configured!");

            var validPorts = new[] {25, 465, 587, 2525};

            if (!validPorts.Contains(mailConfig.Port))
                throw new ApplicationException(
                    $"{nameof(mailConfig.Port)} in {MailOptions.MailSettings} is misconfigured!");

            if (string.IsNullOrWhiteSpace(mailConfig.Password))
                throw new ApplicationException(
                    $"{nameof(mailConfig.Password)} in {MailOptions.MailSettings} is not configured!");
        }

        public static void ValidateUrlsOptions(IOptions<UrlsOptions> urlsOptions)
        {
            var urlsConfig = urlsOptions.Value;

            if (string.IsNullOrWhiteSpace(urlsConfig.Client) ||
                !Uri.IsWellFormedUriString(urlsConfig.Client, UriKind.Absolute))
                throw new ApplicationException($"{nameof(urlsConfig.Client)} in {UrlsOptions.Urls} is not configured!");

            if (string.IsNullOrWhiteSpace(urlsConfig.Idp) ||
                !Uri.IsWellFormedUriString(urlsConfig.Idp, UriKind.Absolute))
                throw new ApplicationException($"{nameof(urlsConfig.Idp)} in {UrlsOptions.Urls} is not configured!");
        }
    }
}