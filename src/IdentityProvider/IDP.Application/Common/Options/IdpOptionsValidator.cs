using System;
using Microsoft.Extensions.Options;

namespace IDP.Application.Common.Options
{
    public static class IdpOptionsValidator
    {
        public static void ValidateSecurityCodeOptions(IOptions<SecurityCodeOptions> options)
        {
            SecurityCodeOptions config;

            try
            {
                config = options.Value;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    $"One of values in {SecurityCodeOptions.SecurityCode} is in incorrect format!", ex);
            }

            if (config.AntiSpamInMinutes < 1 || config.AntiSpamInMinutes > 30)
                throw new ApplicationException(
                    $"{nameof(config.AntiSpamInMinutes)} in {SecurityCodeOptions.SecurityCode} misconfigured!");

            if (config.ExpirationTimeInMinutes < 1 || config.ExpirationTimeInMinutes > 45000)
                throw new ApplicationException(
                    $"{nameof(config.ExpirationTimeInMinutes)} in {SecurityCodeOptions.SecurityCode} misconfigured!");
        }
    }
}