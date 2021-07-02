using System;

namespace IDP.Application.Common.Options
{
    public static class IdpOptionsValidator
    {
        public static void ValidateSecurityCodeOptions(SecurityCodeOptions config)
        {
            if (config.AntiSpamInMinutes < 1 || config.AntiSpamInMinutes > 30)
                throw new ApplicationException(
                    $"{nameof(config.AntiSpamInMinutes)} in {SecurityCodeOptions.SecurityCode} misconfigured!");

            if (config.ExpirationTimeInHours < 1 || config.ExpirationTimeInHours > 45000)
                throw new ApplicationException(
                    $"{nameof(config.ExpirationTimeInHours)} in {SecurityCodeOptions.SecurityCode} misconfigured!");
        }
    }
}