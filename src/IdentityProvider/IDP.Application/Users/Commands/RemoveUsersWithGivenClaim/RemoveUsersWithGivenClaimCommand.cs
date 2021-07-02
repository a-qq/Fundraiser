using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using IDP.Application.Common.Abstractions;
using IDP.Application.Common.Interfaces;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IDP.Application.Users.Commands.RemoveUsersWithGivenClaim
{
    internal sealed class RemoveUsersWithGivenClaimCommand : IInternalCommand
    {
        public string Type { get; }
        public string Value { get; }

        public RemoveUsersWithGivenClaimCommand(string type, string value)
        {
            Type = type;
            Value = value;
        }
    }

    internal sealed class RemoveUsersWithClaimCommandHandler : IRequestHandler<RemoveUsersWithGivenClaimCommand, Result>
    {
        private readonly IUserRepository _userRepository;

        public RemoveUsersWithClaimCommandHandler(IUserRepository userRepository)
        {
            _userRepository = Guard.Against.Null(userRepository, nameof(userRepository));
        }

        public async Task<Result> Handle(RemoveUsersWithGivenClaimCommand request, CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetUsersByClaimValue(request.Type, request.Value, cancellationToken);

            if(users.Any())
                _userRepository.Remove(users);

            return Result.Success();
        } 
    }
}