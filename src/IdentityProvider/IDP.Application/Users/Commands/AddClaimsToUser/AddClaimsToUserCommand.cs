using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using IDP.Application.Common.Abstractions;
using IDP.Application.Common.Interfaces;
using IDP.Application.Common.Models;
using IDP.Domain.UserAggregate.ValueObjects;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IDP.Application.Users.Commands.AddClaimsToUser
{
    internal sealed class AddClaimsToUserCommand : IInternalCommand
    {
        public string Subject { get; }
        public IEnumerable<ClaimInsertModel> Claims { get; }

        public AddClaimsToUserCommand(string subject, IEnumerable<ClaimInsertModel> claims)
        {
            Claims = claims;
            Subject = subject;
        }
    }

    internal sealed class AddClaimsToUserCommandHandler : IRequestHandler<AddClaimsToUserCommand, Result>
    {
        private readonly IUserRepository _userRepository;

        public AddClaimsToUserCommandHandler(
            IUserRepository userRepository)
        {
            _userRepository = Guard.Against.Null(userRepository, nameof(userRepository));
        }

        public async Task<Result> Handle(AddClaimsToUserCommand request, CancellationToken cancellationToken)
        {
            var subject = Subject.Create(request.Subject).Value;

            var userOrNone = await _userRepository.GetUserBySubjectAsync(subject, cancellationToken);

            if (userOrNone.HasNoValue)
                return IdentityProviderApplicationError.User.NotFound(subject);

            var result = Result.Success();

            foreach (var claim in request.Claims)
                result = Result.Combine(result, userOrNone.Value.AddClaim(claim.Type, claim.Value));

            return result;
        }
    }
}