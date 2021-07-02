using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using Destructurama.Attributed;
using IDP.Application.Common.Abstractions;
using IDP.Application.Common.Interfaces;
using IDP.Application.Common.Models;
using IDP.Domain.UserAggregate.Entities;
using IDP.Domain.UserAggregate.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SharedKernel.Infrastructure.Abstractions.Common;
using System.Threading;
using System.Threading.Tasks;

namespace IDP.Application.Users.Commands.ResetPassword
{
    public sealed class ResetPasswordCommand : IUserCommand
    {
        public string SecurityCode { get; }
        [NotLogged]
        public string NewPassword { get; }

        public ResetPasswordCommand(string securityCode, string newPassword)
        {
            SecurityCode = securityCode;
            NewPassword = newPassword;
        }
    }

    internal sealed class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, Result<Unit>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IDateTime _dateTime;

        public ResetPasswordHandler(
            IUserRepository userRepository,
            IPasswordHasher<User> passwordHasher,
            IDateTime dateTime)
        {
            _userRepository = Guard.Against.Null(userRepository, nameof(userRepository));
            _passwordHasher = Guard.Against.Null(passwordHasher, nameof(passwordHasher));
            _dateTime = Guard.Against.Null(dateTime, nameof(dateTime));
        }

        public async Task<Result<Unit>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var userOrNone = await _userRepository.GetUserBySecurityCodeAsync(request.SecurityCode, cancellationToken);

            if (userOrNone.HasNoValue)
                return IdentityProviderApplicationError.User.InvalidSecurityCode.ConvertFailure<Unit>();

            var hashedPassword = HashedPassword.Create(request.NewPassword, _passwordHasher).Value;

            var result = userOrNone.Value.ResetPassword(hashedPassword, _dateTime.Now);

            if (result.IsFailure)
                return result.ConvertFailure<Unit>();

            return Unit.Value;
        }
    }
}