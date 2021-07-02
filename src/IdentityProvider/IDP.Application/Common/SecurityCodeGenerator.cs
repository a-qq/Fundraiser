using System;
using System.Security.Cryptography;
using Ardalis.GuardClauses;
using IDP.Domain.UserAggregate.ValueObjects;

namespace IDP.Application.Common
{
    internal static class SecurityCodeGenerator
    {
        public static SecurityCode GetNewSecurityCode(RNGCryptoServiceProvider generator, HoursToExpire hoursToExpire, DateTime now)
        {
            Guard.Against.Null(hoursToExpire, nameof(hoursToExpire));
            Guard.Against.Null(generator, nameof(generator));
            Guard.Against.InvalidInput(now, nameof(now), c => c <= DateTime.UtcNow);

            var securityCodeData = new byte[128];
            generator.GetBytes(securityCodeData);
            string code = Convert.ToBase64String(securityCodeData);

            return SecurityCode.Create(code, hoursToExpire, now).Value;
        }
    }
}
