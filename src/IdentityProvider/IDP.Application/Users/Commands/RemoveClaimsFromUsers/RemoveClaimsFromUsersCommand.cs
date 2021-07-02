using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using IDP.Application.Common.Abstractions;
using IDP.Application.Users.Commands.RemoveClaimsFromUser;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IDP.Application.Users.Commands.RemoveClaimsFromUsers
{
    internal sealed class RemoveClaimsFromUsersCommand : IInternalCommand
    {
        public IEnumerable<RemoveClaimsFromUserCommand> RemoveClaimsFromUserCommands { get; }

        public RemoveClaimsFromUsersCommand(
            IReadOnlyCollection<RemoveClaimsFromUserCommand> removeClaimsFromUserCommands)
        {
            var duplicates = removeClaimsFromUserCommands.GroupBy(c => c.Subject).Where(g => g.Count() > 1).ToList();
            var commands = removeClaimsFromUserCommands
                .Except(duplicates.SelectMany(x => x))
                .Concat(duplicates.Select(dg => new RemoveClaimsFromUserCommand(
                    dg.Key, dg.SelectMany(d => d.ClaimSpecifications).ToHashSet())));

            RemoveClaimsFromUserCommands = commands;
        }
    }

    internal sealed class RemoveClaimsFromUsersCommandHandler : IRequestHandler<RemoveClaimsFromUsersCommand, Result>
    {
        private readonly ISender _mediator;

        public RemoveClaimsFromUsersCommandHandler(ISender mediator)
        {
            _mediator = Guard.Against.Null(mediator, nameof(mediator));
        }

        public async Task<Result> Handle(RemoveClaimsFromUsersCommand request, CancellationToken cancellationToken)
        {
            var result = Result.Success();
            foreach (var command in request.RemoveClaimsFromUserCommands)
                result = Result.Combine(result, await _mediator.Send(command, cancellationToken));

            return result;
        }
    }
}