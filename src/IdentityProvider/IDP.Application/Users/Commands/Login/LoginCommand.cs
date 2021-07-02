using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using Destructurama.Attributed;
using IDP.Application.Common.Abstractions;
using IDP.Application.Common.Interfaces;
using IDP.Application.Common.Models;
using IDP.Application.DTOs;
using IDP.Domain.UserAggregate.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using SharedKernel.Domain.ValueObjects;
using SharedKernel.Infrastructure.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IDP.Application.Users.Commands.Login
{
    public sealed class LoginCommand : IUserCommand<UserDto>
    {
        public string Email { get; }
        [NotLogged]
        public string Password { get; }

        public LoginCommand(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }

    internal sealed class LoginHandler : IRequestHandler<LoginCommand, Result<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IMemoryCache _cache;

        public LoginHandler(
            IUserRepository userRepository,
            IPasswordHasher<User> passwordHasher,
            IMemoryCache memoryCache)
        {
            _userRepository = Guard.Against.Null(userRepository, nameof(userRepository));
            _passwordHasher = Guard.Against.Null(passwordHasher, nameof(passwordHasher));
            _cache = Guard.Against.Null(memoryCache, nameof(memoryCache));
        }

        public async Task<Result<UserDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var email = Email.Create(request.Email).Value;

            var userOrNone = await _userRepository.GetUserByEmailAsync(email, cancellationToken);

            if (userOrNone.HasNoValue || !userOrNone.Value.HasCompletedRegistration())
                return IdentityProviderApplicationError.User.LoginFailed.ConvertFailure<UserDto>();

            var result = _passwordHasher.VerifyHashedPassword(userOrNone.Value, userOrNone.Value.HashedPassword, request.Password);
            if (result != PasswordVerificationResult.Success)
                return IdentityProviderApplicationError.User.LoginFailed.ConvertFailure<UserDto>();

            if (!userOrNone.Value.IsActive)
                return Result.Failure<UserDto>("Your account has been suspended! Please contact administrator or your unit system supervisor!");

            _cache.Set(SchemaNames.Authentication + userOrNone.Value.Subject, userOrNone.Value,
                new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(new TimeSpan(0, 0, 3))
                    .SetSlidingExpiration(new TimeSpan(0, 0, 2)));

            return new UserDto(userOrNone.Value.Subject, userOrNone.Value.Email);
        }
    }
}