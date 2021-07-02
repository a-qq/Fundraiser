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

namespace IDP.Application.Users.Commands.RemoveClaimsFromUser
{
    internal sealed class RemoveClaimsFromUserCommand : IInternalCommand
    {
        public string Subject { get; }
        public IEnumerable<ClaimDeleteSpecification> ClaimSpecifications { get; }

        public RemoveClaimsFromUserCommand(string subject,
            IEnumerable<ClaimDeleteSpecification> claimSpecifications)
        {
            Subject = subject;
            ClaimSpecifications = claimSpecifications;
        }
    }


    internal sealed class RemoveClaimsFromUserCommandHandler : IRequestHandler<RemoveClaimsFromUserCommand, Result>
    {
        private readonly IUserRepository _userRepository;

        public RemoveClaimsFromUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = Guard.Against.Null(userRepository, nameof(userRepository));
        }

        public async Task<Result> Handle(RemoveClaimsFromUserCommand request, CancellationToken cancellationToken)
        {
            var subject = Subject.Create(request.Subject).Value;

            var userOrNone = await _userRepository.GetUserBySubjectAsync(subject, cancellationToken);
            if (userOrNone.HasNoValue)
                return IdentityProviderApplicationError.User.NotFound(subject);

            var claimsToDelete = userOrNone.Value.Claims.Where(c =>
                request.ClaimSpecifications.Any(cs =>
                    cs.Value == c.Value && (!cs.Value.HasValue || cs.Value.Value == c.Value)))
                .ToList();

            foreach (var claim in claimsToDelete)
                userOrNone.Value.RemoveClaim(claim);

            return Result.Success();
        }
    }
}