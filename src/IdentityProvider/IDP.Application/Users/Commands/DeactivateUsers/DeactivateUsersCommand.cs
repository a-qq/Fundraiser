using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using IDP.Application.Common.Abstractions;
using IDP.Application.Users.Commands.DeactivateUser;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IDP.Application.Users.Commands.DeactivateUsers
{
    internal sealed class DeactivateUsersCommand : IInternalCommand
    {
        public IEnumerable<DeactivateUserCommand> UserDeactivationCommands { get; }

        public DeactivateUsersCommand(IEnumerable<DeactivateUserCommand> userDeactivationCommands)
        {
            UserDeactivationCommands = userDeactivationCommands;
        }
    }

    internal sealed class DeactivateUsersCommandHandler : IRequestHandler<DeactivateUsersCommand, Result>
    {
        private readonly ISender _mediator;

        public DeactivateUsersCommandHandler(
            ISender mediator)
        {
            _mediator = Guard.Against.Null(mediator, nameof(mediator));
        }

        public async Task<Result> Handle(DeactivateUsersCommand request, CancellationToken cancellationToken)
        {
            var result = Result.Success();

            foreach (var command in request.UserDeactivationCommands)
                result = Result.Combine(result, await _mediator.Send(command, cancellationToken));

            return result;
        }
    }
}