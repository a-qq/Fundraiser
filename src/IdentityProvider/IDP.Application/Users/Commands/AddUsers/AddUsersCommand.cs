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
using Microsoft.Extensions.Logging;
using SharedKernel.Domain.ValueObjects;
using SharedKernel.Infrastructure.Abstractions.Common;
using SharedKernel.Infrastructure.Concretes.Services;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace IDP.Application.Users.Commands.AddUsers
{
    internal sealed class AddUsersCommand : IInternalCommand
    {
        public IEnumerable<UserInsertModel> Users { get; }

        public int? HoursToExpire { get; }

        public AddUsersCommand(IEnumerable<UserInsertModel> users, int? hoursToExpire = null)
        {
            Users = users;
            HoursToExpire = hoursToExpire;
        }
    }

    internal sealed class UserInsertModel
    {
        public string Email { get; }
        public string Subject { get; }
        public IEnumerable<ClaimInsertModel> Claims { get; }

        public UserInsertModel(
            string email,
            string subject,
            IEnumerable<ClaimInsertModel> claims)
        {
            Email = email;
            Subject = subject;
            Claims = claims;
        }
    }

    internal sealed class AddUsersCommandHandler : IRequestHandler<AddUsersCommand, Result>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;
        private readonly IUserRepository _userRepository;
        private readonly IDateTime _dateTime;

        public AddUsersCommandHandler(
            IUserRepository userRepository,
            IDateTime dateTime,
            ISqlConnectionFactory sqlConnectionFactory)
        {
            _userRepository = Guard.Against.Null(userRepository, nameof(userRepository));
            _dateTime = Guard.Against.Null(dateTime, nameof(dateTime));
            _sqlConnectionFactory = Guard.Against.Null(sqlConnectionFactory, nameof(sqlConnectionFactory));
        }


        public async Task<Result> Handle(AddUsersCommand request, CancellationToken cancellationToken)
        {
            var usersToInsert = request.Users
                .Select(u => new UserInsertModel(
                    Subject.Create(u.Subject).Value,
                    Email.Create(u.Email).Value,
                    u.Claims))
                .ToList();

            var hoursToExpire = request.HoursToExpire.HasValue
                ? HoursToExpire.Create(request.HoursToExpire.Value).Value
                : HoursToExpire.Infinite;

            IEnumerable<string> userEmails;
            IEnumerable<string> userSubjects;

            using (var connection = _sqlConnectionFactory.GetOpenConnection())
            {
                const string sqlEmails = "SELECT [User].[Email] " +
                                         "WHERE [User].[Email] IN @Emails " +
                                         "FROM [auth].[Users] AS [User]";

                const string sqlSubjects = "SELECT [User].[Subject] " +
                                           "WHERE [User].[Subject] IN @Subjects " +
                                           "FROM [auth].[Users] AS [User]";

                userEmails = await connection.QueryAsync<string>(sqlEmails,
                    new
                    {
                        Emails = request.Users.Select(u => u.Email)
                    });

                userSubjects = await connection.QueryAsync<string>(sqlSubjects,
                    new
                    {
                        Subjects = request.Users.Select(u => u.Subject)
                    });
            }

            var result = Result.Success();
            foreach (var email in userEmails)
                Result.Combine(result, Result.Failure($"User with email '{email}' already exists!"));

            foreach (var subject in userSubjects)
                Result.Combine(result, Result.Failure($"User with subject '{subject}' already exists!"));

            if (result.IsFailure)
                return result;

            var now = _dateTime.Now;

            using (var randomNumberGenerator = new RNGCryptoServiceProvider())
            {
                
                foreach (var userModel in usersToInsert)
                {
                    var securityCode = SecurityCodeGenerator.GetNewSecurityCode(randomNumberGenerator, hoursToExpire, now);
                    var user = new User(userModel.Email, userModel.Subject, securityCode);
                    _userRepository.Add(user);

                    foreach (var claim in userModel.Claims)
                        result = Result.Combine(result, user.AddClaim(claim.Type, claim.Value));
                }
            }

            return result;
        }

        private class UserInsertModel
        {
            public Subject Subject { get; }
            public Email Email { get; }
            public IEnumerable<ClaimInsertModel> Claims { get; }

            public UserInsertModel(Subject subject, Email email, IEnumerable<ClaimInsertModel> claims)
            {
                Subject = subject;
                Email = email;
                Claims = claims;
            }
        }

        internal sealed class AddUsersIdentifiedCommandHandler : IdentifiedCommandHandler<AddUsersCommand>
        {
            public AddUsersIdentifiedCommandHandler(
                ISender mediator,
                IRequestManager requestManager,
                ILogger<IdentifiedCommandHandler<AddUsersCommand>> logger)
                : base(mediator, requestManager, logger)
            {
            }
        }
    }
}