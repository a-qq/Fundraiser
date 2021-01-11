using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.Utils;
using IDP.Core;
using IDP.Core.Interfaces;
using IDP.Core.UserAggregate.Entities;
using IDP.Core.UserAggregate.ValueObjects;
using IDP.Infrastructure.Database;
using IDP.Infrastructure.DTOs;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace IDP.Infrastructure.Services
{
    internal sealed class LocalUserService : ILocalUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IdentityDbContext _identityContext;

        public LocalUserService(
            IUserRepository userRepository,
            IPasswordHasher<User> passwordHasher,
            IdentityDbContext identityContext)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _identityContext = identityContext;
        }
        public async Task<Result<UserDTO>> Login(string email, string password)
        {
            Email userEmail = Email.Create(email).Value;

            Maybe<User> userOrNull = await _userRepository.GetUserByEmailAsync(userEmail);

            if (userOrNull.HasNoValue)
                return Result.Failure<UserDTO>(IDPErrors.User.LoginFailed);

            if (userOrNull.Value.HashedPassword == null)
                return Result.Failure<UserDTO>(IDPErrors.User.RegistrationNotCompelted);

            var result = _passwordHasher.VerifyHashedPassword(userOrNull.Value, userOrNull.Value.HashedPassword.Value, password);
            if (result != PasswordVerificationResult.Success)
                return Result.Failure<UserDTO>(IDPErrors.User.LoginFailed);

            if (!userOrNull.Value.IsActive)
                return Result.Failure<UserDTO>("Your account has been suspended! Please contact headmaster or administrator!");

            return Result.Success(
                new UserDTO(userOrNull.Value.Subject, userOrNull.Value.Email.Value));
        }

        public async Task<Result> CompleteRegistration(string securityCode, string password)
        {
            Maybe<User> userOrNull = await _userRepository.GetUserBySecurityCodeAsync(securityCode);

            if (userOrNull.HasNoValue)
                return Result.Failure(IDPErrors.User.InvalidSecurityCode);

            HashedPassword hashedPassword = HashedPassword.Create(password, _passwordHasher).Value;
            var result = userOrNull.Value.CompleteRegisteration(securityCode, hashedPassword);

            if (result.IsFailure)
                return result;

            await _identityContext.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> ResetPassword(string securityCode, string password)
        {
            Maybe<User> userOrNull = await _userRepository.GetUserBySecurityCodeAsync(securityCode);

            if (userOrNull.HasNoValue)
                return Result.Failure(IDPErrors.User.InvalidSecurityCode);

            HashedPassword hashedPassword = HashedPassword.Create(password, _passwordHasher).Value;
            var result = userOrNull.Value.ResetPassword(securityCode, hashedPassword);

            if (result.IsFailure)
                return result;

            await _identityContext.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> ChangePassword(string email, string oldPassword, string newPassword)
        {
            Email emailObj = Email.Create(email).Value;
            Maybe<User> userOrNull = await _userRepository.GetUserByEmailAsync(emailObj);

            if (userOrNull.HasNoValue)
                return Result.Failure("Please re-login and try again, your password did not change!");

            HashedPassword hashedNewPassword = HashedPassword.Create(newPassword, _passwordHasher).Value;
           // HashedPassword hashedOldPassword = HashedPassword.CheckHashAndConvert(_passwordHasher.HashPassword(userOrNull.Value, oldPassword));

            var result = userOrNull.Value.ChangePassword(oldPassword, hashedNewPassword, _passwordHasher);
            if (result.IsFailure)
                return result;

            await _identityContext.SaveChangesAsync();

            return Result.Success();
        }

        public async Task SendResetPasswordEmail(string email)
        {
            Email emailObj = Email.Create(email).Value;

            Maybe<User> userOrNull = await _userRepository.GetUserByEmailAsync(emailObj);

            if (userOrNull.HasNoValue)
                return; 

            if (!userOrNull.Value.RenewSecurityCode())
                return;

            await _identityContext.SaveChangesAsync();

            return;
        }
    }
}
