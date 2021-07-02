using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using IdentityModel;
using IDP.Application.Common;
using IDP.Application.Common.Abstractions;
using IDP.Application.Common.Interfaces;
using IDP.Application.Common.Models;
using IDP.Application.Common.Options;
using IDP.Domain.UserAggregate.Entities;
using IDP.Domain.UserAggregate.ValueObjects;
using MediatR;
using SharedKernel.Infrastructure.Abstractions.Common;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace IDP.Application.Users.Commands.SendCompleteRegistrationEmail
{
    internal sealed class SendCompleteRegistrationEmailCommand : IInternalCommand
    {
        public string Subject { get; }


        public SendCompleteRegistrationEmailCommand(string subject)
        {
            Subject = subject;
        }
    }

    internal sealed class SendCompleteRegistrationEmailCommandHandler : IRequestHandler<SendCompleteRegistrationEmailCommand, Result>
    {
        private readonly IDateTime _dateTime;
        private readonly IIdpMailManager _mailManager;
        private readonly IUserRepository _userRepository;
        private readonly SecurityCodeOptions _settings;

        public SendCompleteRegistrationEmailCommandHandler(
            IIdpMailManager idpMailManager,
            IUserRepository userRepository,
            IDateTime dateTime,
            SecurityCodeOptions securityCodeOptions)
        {
            _dateTime = Guard.Against.Null(dateTime, nameof(dateTime));
            _userRepository = Guard.Against.Null(userRepository, nameof(userRepository));
            _mailManager = Guard.Against.Null(idpMailManager, nameof(idpMailManager));
            _settings = Guard.Against.Null(securityCodeOptions, nameof(securityCodeOptions));
        }

        public async Task<Result> Handle(SendCompleteRegistrationEmailCommand request, CancellationToken cancellationToken)
        {
            var subject = Subject.Create(request.Subject).Value;

            var userOrNone = await _userRepository.GetUserBySubjectAsync(subject, cancellationToken);
            if (userOrNone.HasNoValue)
                return IdentityProviderApplicationError.User.NotFound(subject);

            var user = userOrNone.Value;

            if (user.SecurityCode is null)
                return Result.Failure($"Security code of user '{subject}' is already consumed!");

            var now = _dateTime.Now;

            if (user.SecurityCode.IsExpired(now.AddHours(2))
                && user.SecurityCode.IssuedAt.AddMinutes(_settings.AntiSpamInMinutes) < now)
            {
                SecurityCode securityCode;
                using (var randomNumberGenerator = new RNGCryptoServiceProvider())
                {
                    securityCode = SecurityCodeGenerator.GetNewSecurityCode(
                        randomNumberGenerator, user.SecurityCode.HoursToExpire, now);
                }

                var result = userOrNone.Value.RequestRegistrationResend(
                    securityCode, _settings.AntiSpamInMinutes, now);

                return result;
            }

            var firstName = user.Claims.FirstOrDefault(
                c => c.Type == JwtClaimTypes.GivenName)?.Value ?? nameof(User);

            await _mailManager.SendRegistrationEmailAsync(user.Email, user.SecurityCode, firstName);

            return Result.Success();
        }
    }
}