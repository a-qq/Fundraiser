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

namespace IDP.Application.Users.Commands.CompleteRegistration
{
    public sealed class CompleteRegistrationCommand : IUserCommand
    {
        public string SecurityCode { get; }
        [NotLogged]
        public string Password { get; }

        public CompleteRegistrationCommand(string securityCode, string password)
        {
            SecurityCode = securityCode;
            Password = password;
        }
    }

    internal sealed class CompleteRegistrationHandler : IRequestHandler<CompleteRegistrationCommand, Result<Unit>>
    {
        private readonly IDateTime _dateTime;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;

        public CompleteRegistrationHandler(
            IUserRepository userRepository,
            IPasswordHasher<User> passwordHasher,
            IDateTime dateTime)
        {
            _dateTime = Guard.Against.Null(dateTime, nameof(dateTime));
            _userRepository = Guard.Against.Null(userRepository, nameof(userRepository));
            _passwordHasher = Guard.Against.Null(passwordHasher, nameof(passwordHasher));
        }
        public async Task<Result<Unit>> Handle(CompleteRegistrationCommand request, CancellationToken cancellationToken)
        {
            var userOrNone = await _userRepository.GetUserBySecurityCodeAsync(request.SecurityCode, cancellationToken);

            if (userOrNone.HasNoValue)
                return IdentityProviderApplicationError.User.InvalidSecurityCode.ConvertFailure<Unit>();

            var hashedPassword = HashedPassword.Create(request.Password, _passwordHasher).Value;

            var result = userOrNone.Value.CompleteRegistration(hashedPassword, _dateTime.Now);

            if (result.IsFailure)
                return result.ConvertFailure<Unit>();

            return Unit.Value;
        }
    }
}