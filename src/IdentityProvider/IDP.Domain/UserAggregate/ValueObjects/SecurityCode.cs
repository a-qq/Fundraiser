using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;

namespace IDP.Domain.UserAggregate.ValueObjects
{
    public class SecurityCode : ValueObject
    {
        private readonly int? _hoursToExpire;
        private readonly DateTime? _issuedAt;

        protected SecurityCode()
        {
        }

        private SecurityCode(string securityCode, HoursToExpire hoursToExpire, DateTime issuedAt)
            : this()
        {
            Value = securityCode;
            _hoursToExpire = hoursToExpire ?? throw new ArgumentNullException(nameof(hoursToExpire));
            _issuedAt = issuedAt;
        }

        public string Value { get; }

        public HoursToExpire HoursToExpire => _hoursToExpire.HasValue
            ? HoursToExpire.Create(_hoursToExpire.Value).Value
            : HoursToExpire.Infinite;

        public DateTime? ExpirationDate => _hoursToExpire.HasValue ? _issuedAt?.AddHours(_hoursToExpire.Value) : null; 

        public bool IsExpired(DateTime now)
            => _hoursToExpire.HasValue && IssuedAt.AddHours(_hoursToExpire.Value) < now;

        public DateTime IssuedAt => _issuedAt.Value;

        public static Result<SecurityCode> Create(string securityCode, HoursToExpire hoursToExpire,
            DateTime issuedAt)
        {
            var validationResult = Validate(securityCode, hoursToExpire, issuedAt);
            if (validationResult.IsFailure)
                return validationResult.ConvertFailure<SecurityCode>();

            return Result.Success(new SecurityCode(securityCode, hoursToExpire, issuedAt));
        }


        private static Result Validate(string securityCode, HoursToExpire hoursToExpire,
            DateTime issuedAt)
        {
            return Result.Combine(ValidateCode(securityCode),
                Result.FailureIf(issuedAt > DateTime.UtcNow, "Security code's issue date is invalid"),
                Result.FailureIf(hoursToExpire is null, "Number of hours to code expiration are required!"));
        }

        public static Result ValidateCode(string securityCode, string propertyName = nameof(SecurityCode))
        {
            return Result.FailureIf(
                string.IsNullOrEmpty(securityCode)
                || !Convert.TryFromBase64String(securityCode, _ = new byte[securityCode.Length * 3 / 4], out _),
                $"{propertyName} is invalid");
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
            yield return IsExpired(DateTime.UtcNow);
        }

        public static implicit operator string(SecurityCode code)
        {
            return code.Value;
        }

        public override string ToString()
        {
            return this.Value;
        }
    }
}