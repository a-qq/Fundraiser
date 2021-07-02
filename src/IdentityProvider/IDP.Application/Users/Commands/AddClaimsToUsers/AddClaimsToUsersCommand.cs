using CSharpFunctionalExtensions;
using IDP.Application.Common.Abstractions;
using IDP.Application.Users.Commands.AddClaimsToUser;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IDP.Application.Users.Commands.AddClaimsToUsers
{
    internal sealed class AddClaimsToUsersCommand : IInternalCommand
    {
        public IEnumerable<AddClaimsToUserCommand> AddClaimsToUserCommands { get; }

        public AddClaimsToUsersCommand(IEnumerable<AddClaimsToUserCommand> addClaimsToUserCommands)
        {
            AddClaimsToUserCommands = addClaimsToUserCommands;
        }
    }

    internal sealed class AddClaimsToUsersCommandHandler : IRequestHandler<AddClaimsToUsersCommand, Result>
    {
        private readonly ISender _mediator;

        public AddClaimsToUsersCommandHandler(ISender mediator)
        {
            _mediator = mediator;
        }


        public async Task<Result> Handle(AddClaimsToUsersCommand request, CancellationToken cancellationToken)
        {
            var result = Result.Success();

            foreach (var command in request.AddClaimsToUserCommands)
                result = Result.Combine(result, await _mediator.Send(command, cancellationToken));
            
            return result;
        }
    }
}