using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using IDP.Application.Common;
using IDP.Application.Common.Abstractions;
using IDP.Application.Common.Interfaces;
using IDP.Application.Common.Models;
using IDP.Application.Common.Options;
using IDP.Domain.UserAggregate.ValueObjects;
using MediatR;
using SharedKernel.Infrastructure.Abstractions.Common;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace IDP.Application.Users.Commands.SendResetPasswordEmail
{
    internal sealed class SendResetPasswordEmailCommand : IInternalCommand
    {
        public string Subject { get; }


        public SendResetPasswordEmailCommand(string subject)
        {
            Subject = subject;
        }
    }

    internal sealed class SendResetPasswordEmailCommandHandler : IRequestHandler<SendResetPasswordEmailCommand, Result>
    {
        private readonly SecurityCodeOptions _settings;
        private readonly IUserRepository _userRepository;
        private readonly IIdpMailManager _mailManager;
        private readonly IDateTime _dateTime;

        public SendResetPasswordEmailCommandHandler(
            IIdpMailManager idpMailManager,
            IUserRepository userRepository,
            IDateTime dateTime,
            SecurityCodeOptions securityCodeOptions)
        {
            _settings = Guard.Against.Null(securityCodeOptions, nameof(securityCodeOptions));
            _dateTime = Guard.Against.Null(dateTime, nameof(dateTime));
            _userRepository = Guard.Against.Null(userRepository, nameof(userRepository));
            _mailManager = Guard.Against.Null(idpMailManager, nameof(idpMailManager));
        }


        public async Task<Result> Handle(SendResetPasswordEmailCommand request, CancellationToken cancellationToken)
        {
            var subject = Subject.Create(request.Subject).Value;

            var userOrNone = await _userRepository.GetUserBySubjectAsync(subject, cancellationToken);
            if (userOrNone.HasNoValue)
                return IdentityProviderApplicationError.User.NotFound(subject);

            var user = userOrNone.Value;

            if (user.SecurityCode is null)
                return Result.Failure($"Security code of user '{subject}' is already consumed!");

            var now = _dateTime.Now;
            
            if (user.SecurityCode.IsExpired(now.AddHours(12))
                && user.SecurityCode.IssuedAt.AddMinutes(_settings.AntiSpamInMinutes) < now)
            {
                SecurityCode securityCode;
                using (var randomNumberGenerator = new RNGCryptoServiceProvider())
                {
                    securityCode = SecurityCodeGenerator.GetNewSecurityCode(randomNumberGenerator, user.SecurityCode.HoursToExpire, now);
                }

                var result = userOrNone.Value.RequestPasswordReset(securityCode, now, _settings.AntiSpamInMinutes);

                return result;
            }

            await _mailManager.SendResetPasswordEmail(user.Email, user.SecurityCode);

            return Result.Success();
        }
    }
}