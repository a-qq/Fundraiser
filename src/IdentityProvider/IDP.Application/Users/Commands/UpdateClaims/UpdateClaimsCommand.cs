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

namespace IDP.Application.Users.Commands.UpdateClaims
{
    internal sealed class UpdateClaimsCommand : IInternalCommand
    {
        public UpdateClaimsCommand(string subject, IEnumerable<ClaimUpdateSpecification> claimSpecifications)
        {
            Subject = subject;
            ClaimSpecifications = claimSpecifications;
        }

        public string Subject { get; }
        public IEnumerable<ClaimUpdateSpecification> ClaimSpecifications { get; }
    }

    internal sealed class UpdateClaimsCommandHandler : IRequestHandler<UpdateClaimsCommand, Result>
    {
        private readonly IUserRepository _userRepository;

        public UpdateClaimsCommandHandler(IUserRepository userRepository)
        {
            _userRepository = Guard.Against.Null(userRepository, nameof(userRepository));
        }


        public async Task<Result> Handle(UpdateClaimsCommand request, CancellationToken cancellationToken)
        {
            var subject = Subject.Create(request.Subject).Value;

            var userOrNone = await _userRepository.GetUserBySubjectAsync(subject, cancellationToken);
            if (userOrNone.HasNoValue)
                return IdentityProviderApplicationError.User.NotFound(subject);

            foreach (var claimSpecification in request.ClaimSpecifications)
            {
                if (claimSpecification.OldValue.HasValue)
                    userOrNone.Value.UpdateClaim(claimSpecification.Type, claimSpecification.NewValue, claimSpecification.OldValue.Value);
                else 
                    userOrNone.Value.UpdateClaims(claimSpecification.Type, claimSpecification.NewValue);
            }
            
            return Result.Success();
        }
    }
}