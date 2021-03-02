using CSharpFunctionalExtensions;
using IDP.Application.Common.Interfaces;
using IDP.Application.Common.Options;
using IDP.Application.DTOs;
using IDP.Domain;
using IDP.Domain.UserAggregate.Entities;
using IDP.Domain.UserAggregate.ValueObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using SharedKernel.Domain.Errors;
using SharedKernel.Domain.Utils;
using SharedKernel.Domain.ValueObjects;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace IDP.Application.Users
{
    internal sealed class LocalUserService : ILocalUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IIdentityContext _identityContext;
        private readonly IMemoryCache _cache;
        private readonly SecurityCodeOptions _codeOptions;

        public LocalUserService(
            IUserRepository userRepository,
            IPasswordHasher<User> passwordHasher,
            IIdentityContext identityContext,
            IMemoryCache memoryCache,
            IOptions<SecurityCodeOptions> codeOptions)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _identityContext = identityContext;
            _cache = memoryCache;
            _codeOptions = codeOptions.Value;
        }

        public async Task<Result<UserDTO, Error>> Login(string userEmail, string password)
        {
            Email email = Email.Create(userEmail).Value;

            Maybe<User> userOrNone = await _userRepository.GetUserByEmailAsync(email);

            if (userOrNone.HasNoValue)
                return IDPError.User.LoginFailed();

            var isRegistered = userOrNone.Value.HasCompletedRegistration();
            if (isRegistered.IsFailure)
                return isRegistered.Error;

            var result = _passwordHasher.VerifyHashedPassword(userOrNone.Value, userOrNone.Value.HashedPassword, password);
            if (result != PasswordVerificationResult.Success)
                return IDPError.User.LoginFailed();

            if (!userOrNone.Value.IsActive)
                return new Error("Your account has been suspended! Please contact administrator or your unit system supervisior!");

            _cache.Set(SchemaNames.Authentiaction + userOrNone.Value.Subject, userOrNone.Value, new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(new TimeSpan(0, 0, 3))
                .SetSlidingExpiration(new TimeSpan(0, 0, 2)));

            return new UserDTO(userOrNone.Value.Subject, userOrNone.Value.Email);
        }

        public async Task<Result<bool, Error>> CompleteRegistration(string securityCode, string password)
        {
            var userOrNone = await _userRepository.GetUserBySecurityCodeAsync(securityCode);

            if (userOrNone.HasNoValue)
                return IDPError.User.InvalidSecurityCode();

            HashedPassword hashedPassword = HashedPassword.Create(password, _passwordHasher).Value;

            var result = userOrNone.Value.CompleteRegisteration(hashedPassword, DateTime.UtcNow);

            if (result.IsFailure)
                return result;

            await _identityContext.SaveChangesAsync();

            return Result.Success<bool, Error>(true);
        }

        public async Task<Result<bool, Error>> ResetPassword(string securityCode, string password)
        {
            var userOrNone = await _userRepository.GetUserBySecurityCodeAsync(securityCode);

            if (userOrNone.HasNoValue)
                return IDPError.User.InvalidSecurityCode();

            HashedPassword hashedPassword = HashedPassword.Create(password, _passwordHasher).Value;

            var result = userOrNone.Value.ResetPassword(hashedPassword, DateTime.UtcNow);

            if (result.IsFailure)
                return result;

            await _identityContext.SaveChangesAsync();

            return Result.Success<bool, Error>(true);
        }

        public async Task<Result<bool, Error>> ChangePassword(string userEmail, string oldPassword, string newPassword)
        {
            Email email = Email.Create(userEmail).Value;
            Maybe<User> userOrNone = await _userRepository.GetUserByEmailAsync(email);

            if (userOrNone.HasNoValue)
                return new Error("Please re-login and try again, your password was not changed!");

            HashedPassword hashedNewPassword = HashedPassword.Create(newPassword, _passwordHasher).Value;

            var result = userOrNone.Value.ChangePassword(oldPassword, hashedNewPassword, _passwordHasher);

            if (result.IsFailure)
                return result;

            await _identityContext.SaveChangesAsync();

            return Result.Success<bool, Error>(true);
        }

        public async Task RequestPasswordReset(string userEmail)
        {
            Email email = Email.Create(userEmail).Value;

            Maybe<User> userOrNone = await _userRepository.GetUserByEmailAsync(email);

            if (userOrNone.HasNoValue)
                return;

            string code = null;

            using (var randomNumberGenerator = new RNGCryptoServiceProvider())
            {
                var securityCodeData = new byte[128];
                randomNumberGenerator.GetBytes(securityCodeData);
                code = Convert.ToBase64String(securityCodeData);
            }

            if (!userOrNone.Value.RenewSecurityCode(DateTime.UtcNow, code, _codeOptions.ExpirationTimeInMinutes))
                return;

            await _identityContext.SaveChangesAsync();

            return;
        }
    }
}
