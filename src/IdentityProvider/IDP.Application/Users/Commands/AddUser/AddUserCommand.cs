using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using Dapper;
using IDP.Application.Common;
using IDP.Application.Common.Abstractions;
using IDP.Application.Common.Interfaces;
using IDP.Application.Common.Models;
using IDP.Domain.UserAggregate.Entities;
using IDP.Domain.UserAggregate.ValueObjects;
using MediatR;
using SharedKernel.Domain.ValueObjects;
using SharedKernel.Infrastructure.Abstractions.Common;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace IDP.Application.Users.Commands.AddUser
{
    internal sealed class AddUserCommand : IInternalCommand
    {
        public string Email { get; }
        public string Subject { get; }
        public int? HoursToExpire { get; }
        public IEnumerable<ClaimInsertModel> Claims { get; }

        public AddUserCommand(string email, string subject, IEnumerable<ClaimInsertModel> claims, int? hoursToExpire = null)
        {
            Email = email;
            Claims = claims;
            HoursToExpire = hoursToExpire;
            Subject = subject;
        }
    }

    internal sealed class AddUserCommandHandler : IRequestHandler<AddUserCommand, Result>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;
        private readonly IUserRepository _userRepository;
        private readonly IDateTime _dateTime;

        public AddUserCommandHandler(
            IUserRepository userRepository,
            IDateTime dateTime,
            ISqlConnectionFactory sqlConnectionFactory)
        {
            _userRepository = Guard.Against.Null(userRepository, nameof(userRepository));
            _dateTime = Guard.Against.Null(dateTime, nameof(dateTime));
            _sqlConnectionFactory = Guard.Against.Null(sqlConnectionFactory, nameof(sqlConnectionFactory));
        }


        public async Task<Result> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            var email = Email.Create(request.Email).Value;
            var subject = Subject.Create(request.Subject).Value;
            var hoursToExpire = request.HoursToExpire.HasValue
                ? HoursToExpire.Create(request.HoursToExpire.Value).Value
                : HoursToExpire.Infinite;

            using (var connection = _sqlConnectionFactory.GetOpenConnection())
            {
                const string sql = "SELECT TOP 1 1" +
                                   "FROM [auth].[Users] AS [User] " +
                                   "WHERE [User].[Email] = @Email " +
                                   "OR [User].[Subject] = @Subject";

                var userNumber = await connection.QuerySingleOrDefaultAsync<int?>(sql,
                    new
                    {
                        Email = request.Email,
                        Subject = request.Subject
                    });

                if (userNumber.HasValue)
                    return Result.Failure($"User with email '{email}' or subject '{subject}' already exists!");
            }

            var now = _dateTime.Now;

            SecurityCode securityCode;
            using (var randomNumberGenerator = new RNGCryptoServiceProvider())
            {
                securityCode = SecurityCodeGenerator.GetNewSecurityCode(randomNumberGenerator, hoursToExpire, now);
            }

            var user = new User(email, subject, securityCode);

            _userRepository.Add(user);

            var result = Result.Success();

            foreach (var claim in request.Claims)
                result = Result.Combine(result, user.AddClaim(claim.Type, claim.Value));

            return result;
        }
    }
}