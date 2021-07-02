using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Infrastructure.Abstractions.Common;
using SharedKernel.Infrastructure.Abstractions.Requests;
using SharedKernel.Infrastructure.Extensions;
using System.Threading;
using System.Threading.Tasks;

namespace SharedKernel.Infrastructure.Concretes.Services
{
    public class IdentifiedCommandHandler<T> : IRequestHandler<IdentifiedCommand<T>, Result>
            where T : IRequest<Result>
    {
        private readonly ISender _mediator;
        private readonly IRequestManager _requestManager;
        private readonly ILogger<IdentifiedCommandHandler<T>> _logger;

        public IdentifiedCommandHandler(
            ISender mediator,
            IRequestManager requestManager,
            ILogger<IdentifiedCommandHandler<T>> logger)
        {
            _mediator = mediator;
            _requestManager = requestManager;
            _logger = logger;
        }


        /// <summary>
        /// This method handles the command. It just ensures that no other request exists with the same ID, and if this is the case
        /// just enqueues the original inner command.
        /// </summary>
        /// <param name="message">IdentifiedCommand which contains both original command & request ID</param>
        /// <returns>Return value of inner command or default value if request same ID was found</returns>
        public async Task<Result> Handle(IdentifiedCommand<T> message, CancellationToken cancellationToken)
        {
            var alreadyExists = await _requestManager.ExistAsync(message.Id);
            if (alreadyExists)
                return Result.Success();

            await _requestManager.CreateRequestForCommandAsync<T>(message.Id);
            var command = message.Command;
            var commandName = command.GetGenericTypeName();
            var commandId = message.Id;


            _logger.LogInformation(
                "----- Sending idempotent command: {CommandName} - Id: {CommandId} ({@Command})",
                commandName,
                commandId,
                command);

            // Send the embedded business command to mediator so it runs its related CommandHandler 
            var result = await _mediator.Send(command, cancellationToken);

            _logger.LogInformation(
                "----- Idempotent command succeeded - {CommandName} - Id: {CommandId} ({@Command})",
                commandName,
                commandId,
                command);

            return result;
        }
    }
}