using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using IDP.Application.Common;
using IDP.Application.Common.Abstractions;
using IDP.Application.Common.Interfaces;
using IDP.Application.Common.Models;
using IDP.Application.Common.Options;
using IDP.Domain.UserAggregate.ValueObjects;
using MediatR;
using SharedKernel.Domain.ValueObjects;
using SharedKernel.Infrastructure.Abstractions.Common;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace IDP.Application.Users.Commands.RequestPasswordReset
{
    public sealed class RequestPasswordResetCommand : IUserCommand
    {
        public string Email { get; }

        public RequestPasswordResetCommand(string email)
        {
            Email = email;
        }
    }

    internal sealed class RequestPasswordResetHandler : IRequestHandler<RequestPasswordResetCommand, Result<Unit>>
    {
        private readonly IDateTime _dateTime;
        private readonly IUserRepository _userRepository;
        private readonly SecurityCodeOptions _settings;

        public RequestPasswordResetHandler(
            IUserRepository userRepository,
            IDateTime dateTime,
            SecurityCodeOptions securityCodeOptions)
        {
            _dateTime = Guard.Against.Null(dateTime, nameof(dateTime));
            _userRepository = Guard.Against.Null(userRepository, nameof(userRepository));
            _settings = Guard.Against.Null(securityCodeOptions, nameof(securityCodeOptions));
        }

        public async Task<Result<Unit>> Handle(RequestPasswordResetCommand request, CancellationToken cancellationToken)
        {
            var email = Email.Create(request.Email).Value;
            var hoursToExpire = HoursToExpire.Create(_settings.ExpirationTimeInHours).Value;

            var userOrNone = await _userRepository.GetUserByEmailAsync(email, cancellationToken);

            if (userOrNone.HasNoValue)
                return IdentityProviderApplicationError.User.NotFound(email).ConvertFailure<Unit>();

            var now = _dateTime.Now;

            SecurityCode securityCode;

            using (var generator = new RNGCryptoServiceProvider())
            {
                securityCode = SecurityCodeGenerator.GetNewSecurityCode(generator, hoursToExpire, now);
            }

            var result = userOrNone.Value.RequestPasswordReset(securityCode, now, _settings.AntiSpamInMinutes);

            if (result.IsFailure)
                return result.ConvertFailure<Unit>();

            return Unit.Value;
        }
    }
}