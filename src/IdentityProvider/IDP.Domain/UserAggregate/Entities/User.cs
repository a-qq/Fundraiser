using CSharpFunctionalExtensions;
using IdentityModel;
using IDP.Domain.UserAggregate.Events;
using IDP.Domain.UserAggregate.ValueObjects;
using Microsoft.AspNetCore.Identity;
using SharedKernel.Domain.Common;
using SharedKernel.Domain.Errors;
using SharedKernel.Domain.Utils;
using SharedKernel.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IDP.Domain.UserAggregate.Entities
{
    public class User : AggregateRoot
    {
        private readonly List<Claim> _claims = new List<Claim>();
        public string Subject { get; }
        public HashedPassword HashedPassword { get; private set; }
        public bool IsActive { get; private set; }
        public Email Email { get; }
        public virtual SecurityCode SecurityCode { get; private set; }
        public virtual IReadOnlyList<Claim> Claims => _claims.AsReadOnly();

        protected User() { }

        public Result<bool, Error> CompleteRegisteration(HashedPassword password, DateTime now)
        {
            if (password is null)
                throw new ArgumentNullException(nameof(password));

            if (!(this.HashedPassword is null))
                return new Error("Registration already completed!");

            if (this.SecurityCode.ExpirationDate.IsExpired(now))
                return IDPError.User.InvalidSecurityCode();

            this.HashedPassword = password;
            this.IsActive = true;
            this.SecurityCode = SecurityCode.None;

            AddDomainEvent(new UserCompletedRegistrationEvent(this.Subject,
                this.Claims.Single(c => c.Type == JwtClaimTypes.GivenName).Value,
                this.Claims.Single(c => c.Type == JwtClaimTypes.FamilyName).Value,
                this.Claims.Single(c => c.Type == JwtClaimTypes.Gender).Value,
                this.Claims.Single(c => c.Type == JwtClaimTypes.Role).Value,
                this.Claims.Single(c => c.Type == CustomClaimTypes.SchoolId).Value,
                this.Claims.SingleOrDefault(c => c.Type == CustomClaimTypes.GroupId)?.Value));

            return Result.Success<bool, Error>(true);
        }

        public Result<bool, Error> ChangePassword(string oldPassword, HashedPassword newPassword, IPasswordHasher<User> passwordHasher)
        {
            if (string.IsNullOrWhiteSpace(oldPassword))
                throw new ArgumentNullException(nameof(oldPassword));

            if (newPassword is null)
                throw new ArgumentNullException(nameof(newPassword));

            if (passwordHasher is null)
                throw new ArgumentNullException(nameof(passwordHasher));

            var isRegistered = HasCompletedRegistration();
            if (isRegistered.IsFailure)
                return isRegistered;

            var result = passwordHasher.VerifyHashedPassword(this, this.HashedPassword.Value, oldPassword);

            if (result == PasswordVerificationResult.Failed)
                return new Error("Old password incorrect!");

            this.HashedPassword = newPassword;

            return Result.Success<bool, Error>(true);
        }

        public Result<bool, Error> ResetPassword(HashedPassword newPassword, DateTime now)
        {
            if (newPassword is null)
                throw new ArgumentNullException(nameof(newPassword));

            if (this.SecurityCode.ExpirationDate.IsExpired(now))
                return IDPError.User.InvalidSecurityCode();

            var isRegistered = HasCompletedRegistration();

            if (isRegistered.IsFailure)
                return isRegistered;


            this.HashedPassword = newPassword;
            this.SecurityCode = SecurityCode.None;

            return Result.Success<bool, Error>(true);
        }

        public bool RenewSecurityCode(DateTime now, string code, int securityCodeExpTimeInMinutes)
        {
            if (!CanRenewSecurityCode(now, securityCodeExpTimeInMinutes))
                return false;

            var expDate = ExpirationDate.Create(now.AddMinutes(securityCodeExpTimeInMinutes)).Value;

            this.SecurityCode = SecurityCode.Create(code, expDate, now).Value;

            AddDomainEvent(new SecurityCodeRenewed(this.Email, this.SecurityCode));

            return true;
        }

        public Result<bool, Error> HasCompletedRegistration()
        {
            if (this.HashedPassword is null)
                return IDPError.User.RegistrationNotCompelted();

            return Result.Success<bool, Error>(true);
        }

        private bool CanRenewSecurityCode(DateTime now, int antiSpamInMinutes)
        {
            if (HasCompletedRegistration().IsFailure)
                return false;

            if (!(this.SecurityCode is null) && this.SecurityCode.IssuedAt.HasValue &&
                this.SecurityCode.IssuedAt.Value.AddMinutes(antiSpamInMinutes) > now)
            {
                return false;
            }

            return true;
        }
    }
}
