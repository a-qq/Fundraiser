using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.Utils;
using System;
using System.Collections.Generic;

namespace IDP.Core.UserAggregate.ValueObjects
{
    public class SecurityCode : ValueObject
    {
        public static readonly SecurityCode None = new SecurityCode(null, ExpirationDate.Infinite, null);

        private readonly DateTime? _expirationDate;
        public string Value { get; }
        public ExpirationDate ExpirationDate => (ExpirationDate)_expirationDate;
        public DateTime? IssuedAt { get; }

        protected SecurityCode()
        {
        }

        private SecurityCode(string securityCode, ExpirationDate expirationDate, DateTime? issuedAt)
            : this()
        {
            Value = securityCode;
            _expirationDate = expirationDate ?? throw new ArgumentNullException(nameof(expirationDate));
            IssuedAt = issuedAt;
        }


        public static Result<SecurityCode, Error> Create(string securityCode, ExpirationDate expirationDate, DateTime? issuedAt = null)
        {
            var validationResult = Validate(securityCode, expirationDate, issuedAt);
            if (validationResult.IsFailure)
                return validationResult.ConvertFailure<SecurityCode>();

            return Result.Success<SecurityCode, Error>(new SecurityCode(securityCode, expirationDate, issuedAt));
        }

        private static Result<bool, Error> Validate(string securityCode, ExpirationDate expirationDate, DateTime? issuedAt)
        {
            return Result.Combine(
                Result.FailureIf(!string.IsNullOrEmpty(securityCode) && !Convert.TryFromBase64String(securityCode, _ = new byte[securityCode.Length * 3 / 4], out _), true, new Error("Security code is invalid")),
                Result.FailureIf((expirationDate != ExpirationDate.Infinite && issuedAt == null) || (issuedAt != null && issuedAt > DateTime.UtcNow),
                    true, new Error("Security code's issue date is invalid")),
                Result.FailureIf(expirationDate < issuedAt, true, new Error("Security code's expiration date is invalid")));
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
            yield return ExpirationDate.IsExpired;
        }

        public static implicit operator string(SecurityCode code)
        {
            return code.Value;
        }
    }
}
