using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using IDP.Domain.UserAggregate.Events;
using IDP.Domain.UserAggregate.ValueObjects;
using Microsoft.AspNetCore.Identity;
using SharedKernel.Domain.Common;
using SharedKernel.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using IDP.Domain.Common.Errors;

namespace IDP.Domain.UserAggregate.Entities
{
    public class User : AggregateRoot
    {
        private readonly List<Claim> _claims = new List<Claim>();

        protected User()
        {
        }

        public User(Email email, Subject subject, SecurityCode securityCode)
        {
            Subject = Guard.Against.Null(subject, nameof(subject));
            Email = Guard.Against.Null(email, nameof(email));
            SecurityCode = Guard.Against.Null(securityCode, nameof(securityCode));
            IsActive = true; //TODO: change to false

            AddDomainEvent(new UserRegistrationRequestedDomainEvent(Subject));
        }

        public Subject Subject { get; }
        public HashedPassword HashedPassword { get; private set; }
        public bool IsActive { get; private set; }
        public Email Email { get; }
        public virtual SecurityCode SecurityCode { get; private set; }
        public virtual IReadOnlyList<Claim> Claims => _claims.AsReadOnly();

        public Result CompleteRegistration(HashedPassword password, DateTime now)
        {
            Guard.Against.Null(password, nameof(password));

            if (HasCompletedRegistration())
                return Result.Failure("Registration already completed!");

            if (SecurityCode.IsExpired(now))
                return IdentityProviderDomainError.User.ExpiredSecurityCode();

            HashedPassword = password;
            IsActive = true;
            SecurityCode = null;

            AddDomainEvent(new UserRegisteredDomainEvent(Subject));

            return Result.Success();
        }

        public Result RequestRegistrationResend(SecurityCode securityCode, int antiSpamInMinutes, DateTime now)
        {
            Guard.Against.NegativeOrZero(antiSpamInMinutes, nameof(antiSpamInMinutes));
            Guard.Against.Null(securityCode, nameof(securityCode));
            if (HasCompletedRegistration())
                return Result.Failure("Registration already completed!");

            if (!HaveAntiSpamIntervalPassed(now, antiSpamInMinutes))
                return IdentityProviderDomainError.User.AntiSpamIntervalNotPassed(antiSpamInMinutes);

            SecurityCode = securityCode;

            AddDomainEvent(new UserRegistrationRequestedDomainEvent(Subject));

            return Result.Success();
        }

        public Result ChangePassword(string oldPassword, HashedPassword newPassword,
            IPasswordHasher<User> passwordHasher)
        {
            Guard.Against.Null(passwordHasher, nameof(passwordHasher));
            Guard.Against.NullOrWhiteSpace(oldPassword, nameof(oldPassword));
            Guard.Against.Null(newPassword, nameof(newPassword));

            if (!HasCompletedRegistration())
                return IdentityProviderDomainError.User.RegistrationNotCompleted();

            var result = passwordHasher.VerifyHashedPassword(
                this, this.HashedPassword, oldPassword);

            if (result == PasswordVerificationResult.Failed)
                return Result.Failure("Old password incorrect!");

            HashedPassword = newPassword;

            return Result.Success();
        }

        public Result ResetPassword(HashedPassword newPassword, DateTime now)
        {
            Guard.Against.Null(newPassword, nameof(newPassword));

            if (SecurityCode.IsExpired(now))
                return IdentityProviderDomainError.User.ExpiredSecurityCode();

            if (!HasCompletedRegistration())
                return IdentityProviderDomainError.User.RegistrationNotCompleted();

            HashedPassword = newPassword;
            SecurityCode = null;

            return Result.Success();
        }

        public Result RequestPasswordReset(SecurityCode securityCode, DateTime now, int antiSpamInMinutes)
        {
            Guard.Against.NegativeOrZero(antiSpamInMinutes, nameof(antiSpamInMinutes));
            Guard.Against.Null(securityCode, nameof(securityCode));

            if (!HasCompletedRegistration())
                return IdentityProviderDomainError.User.RegistrationNotCompleted();

            if (!HaveAntiSpamIntervalPassed(now, antiSpamInMinutes))
                return IdentityProviderDomainError.User.AntiSpamIntervalNotPassed(antiSpamInMinutes);

            this.SecurityCode = securityCode;

            AddDomainEvent(new PasswordResetRequestedDomainEvent(Subject));

            return Result.Success();
        }

        public Result AddClaim(string type, string value)
        {
            Guard.Against.NullOrWhiteSpace(type, nameof(type));
            Guard.Against.NullOrWhiteSpace(value, nameof(value));

            if (_claims.Any(c => c.Type == type && c.Value == value))
                return Result.Failure($"User already have claim (Type: '{type}', Value: '{value}')");

            _claims.Add(new Claim(type, value));

            return Result.Success();
        }

        public void UpdateClaims(string type, string newValue)
        {
            Guard.Against.NullOrWhiteSpace(type, nameof(type));

            var claims = _claims.Where(c => c.Type == type);
            foreach (var claim in claims)
                claim.Update(newValue);
        }

        public void UpdateClaim(string type, string newValue, string oldValue)
        {
            Guard.Against.NullOrWhiteSpace(type, nameof(type));
            Guard.Against.NullOrWhiteSpace(oldValue, nameof(oldValue));

            var claim = _claims.Single(c => c.Type == type && c.Value == oldValue);
            claim.Update(newValue);
        }

        public void RemoveClaim(Claim claim)
        {
            Guard.Against.Null(claim, nameof(claim));

            _claims.Remove(claim);
        }

        public Result Deactivate()
        {
            if (!HasCompletedRegistration())
                return IdentityProviderDomainError.User.RegistrationNotCompleted(this.Subject);

            if (IsActive)
                IsActive = false;

            return Result.Success();
        }

        public Result Reactivate()
        {
            if (!HasCompletedRegistration())
                return IdentityProviderDomainError.User.RegistrationNotCompleted(this.Subject);

            if (!IsActive)
                IsActive = true;

            return Result.Success();
        }

        public bool HasCompletedRegistration()
            => !(this.HashedPassword is null);

        private bool HaveAntiSpamIntervalPassed(DateTime now, int antiSpamInMinutes)
            => SecurityCode?.IssuedAt != null && SecurityCode.IssuedAt.AddMinutes(
                Guard.Against.NegativeOrZero(antiSpamInMinutes, nameof(antiSpamInMinutes))) < now;

    }
}