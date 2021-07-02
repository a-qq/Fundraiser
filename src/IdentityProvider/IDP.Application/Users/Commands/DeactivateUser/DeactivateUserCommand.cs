using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using IDP.Application.Common.Abstractions;
using IDP.Application.Common.Interfaces;
using IDP.Application.Common.Models;
using IDP.Domain.UserAggregate.ValueObjects;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IDP.Application.Users.Commands.DeactivateUser
{
    internal sealed class DeactivateUserCommand : IInternalCommand
    {
        public string Subject { get; }
        public IEnumerable<ClaimDeleteSpecification> ClaimSpecifications { get; }

        public DeactivateUserCommand(string subject, IEnumerable<ClaimDeleteSpecification> claimSpecifications = null)
        {
            Subject = subject;
            ClaimSpecifications = claimSpecifications;
        }
    }

    internal sealed class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand, Result>
    {
        private readonly IUserRepository _userRepository;

        public DeactivateUserCommandHandler(
            IUserRepository userRepository)
        {
            _userRepository = Guard.Against.Null(userRepository, nameof(userRepository));
        }

        public async Task<Result> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
        {
            var subject = Subject.Create(request.Subject).Value;

            var userOrNone = await _userRepository.GetUserBySubjectAsync(subject, cancellationToken);

            if (userOrNone.HasNoValue)
                return IdentityProviderApplicationError.User.NotFound(subject);

            var result = userOrNone.Value.Deactivate();

            var claimsToDelete = userOrNone.Value.Claims
                .Where(c => request.ClaimSpecifications
                    .Any(cs => cs.Value == c.Value && (cs.Value.HasNoValue || cs.Value.Value == c.Value)));

            foreach (var claim in claimsToDelete)
                userOrNone.Value.RemoveClaim(claim);

            return result;
        }
    }
}