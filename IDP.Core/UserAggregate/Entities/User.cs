using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.Utils;
using IdentityModel;
using IDP.Core.UserAggregate.Events;
using IDP.Core.UserAggregate.ValueObjects;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace IDP.Core.UserAggregate.Entities
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

        protected User()
        { 
        }

        public Result CompleteRegisteration(string securityCode, HashedPassword password)
        {
            if (this.HashedPassword != null)
                return Result.Failure("User already registered!");

            if (this.SecurityCode.Value != securityCode || string.IsNullOrWhiteSpace(securityCode))
                throw new InvalidOperationException(nameof(securityCode));

            if (password == null)
                throw new ArgumentNullException(nameof(password));

            this.HashedPassword = password;
            this.IsActive = true;
            this.SecurityCode = SecurityCode.None;

            AddDomainEvent(new UserCompletedRegistrationEvent(this.Subject,
                this.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.GivenName).Value,
                this.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.FamilyName).Value,
                this.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Gender).Value,
                this.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Role).Value,
                this.Claims.FirstOrDefault(c => c.Type == "school_id").Value));

            return Result.Success();
        }

        public Result ChangePassword(string oldPassword, HashedPassword newPassword, IPasswordHasher<User> passwordHasher)
        {
            if (this.HashedPassword == null)
                return Result.Failure(IDPErrors.User.RegistrationNotCompelted);

            if (string.IsNullOrWhiteSpace(oldPassword))
                throw new ArgumentNullException(nameof(oldPassword));

            if (newPassword == null)
                throw new ArgumentNullException(nameof(newPassword));

            if (passwordHasher == null)
                throw new ArgumentNullException(nameof(passwordHasher));

            var result = passwordHasher.VerifyHashedPassword(this, this.HashedPassword.Value, oldPassword);
            if (result == PasswordVerificationResult.Failed)
                return Result.Failure("Old password incorrect!");

            this.HashedPassword = newPassword;

            return Result.Success();
        }

        public Result ResetPassword(string securityCode, HashedPassword newPassword)
        {
            if (this.HashedPassword == null)
                return Result.Failure(IDPErrors.User.RegistrationNotCompelted);

            if (this.SecurityCode.Value != securityCode || string.IsNullOrWhiteSpace(securityCode))
                throw new InvalidOperationException(nameof(securityCode));

            if (this.SecurityCode.ExpirationDate.IsExpired)
                return Result.Failure(IDPErrors.User.InvalidSecurityCode);

            if (newPassword == null)
                throw new ArgumentNullException(nameof(newPassword));

            this.HashedPassword = newPassword;
            this.SecurityCode = SecurityCode.None;

            return Result.Success();
        }

        public bool RenewSecurityCode()
        {
            if (!CanRenewSecurityCode())
                return false;
            
            if (!int.TryParse(Environment.GetEnvironmentVariable("SecurityCodeExpTimeInMinutes"), out int minutes))
                minutes = 120;
            DateTime now = DateTime.UtcNow;

            using (var randomNumberGenerator = new RNGCryptoServiceProvider())
            {
                var securityCodeData = new byte[128];
                randomNumberGenerator.GetBytes(securityCodeData);
                string securityCode = Convert.ToBase64String(securityCodeData);

                SecurityCode securityCodeObj = SecurityCode.Create(securityCode, ExpirationDate.Create(now.AddMinutes(minutes)).Value, now).Value;

                this.SecurityCode = securityCodeObj;
            }
            AddDomainEvent(new SendResetPasswordEmailEvent(this.Email, this.SecurityCode));
            return true;
        }

        private bool CanRenewSecurityCode()
        {
            if(this.HashedPassword == null)
                return false;

            if (!int.TryParse(Environment.GetEnvironmentVariable("AntispamInMinutes"), out int minutes))
                minutes = 3;

            if (this.SecurityCode != null && this.SecurityCode.IssuedAt.HasValue &&
                this.SecurityCode.IssuedAt.Value.AddSeconds(minutes) > DateTime.UtcNow)
                //return Result.Failure($"Reset password email was sent in last {minutes} minutes. Try again later!");
                return false;
            return true;
        }
    }
}
