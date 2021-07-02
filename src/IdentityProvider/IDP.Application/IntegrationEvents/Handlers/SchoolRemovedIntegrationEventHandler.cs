using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using IDP.Application.Common.Models;
using IDP.Application.IntegrationEvents.Events;
using IDP.Application.Users.Commands.RemoveUsersWithGivenClaim;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using SharedKernel.Domain.Utils;
using SharedKernel.Infrastructure.Abstractions.EventBus;
using System.Threading.Tasks;
using static IDP.Application.MediatorModule;

namespace IDP.Application.IntegrationEvents.Handlers
{
    internal sealed class SchoolRemovedIntegrationEventHandler : IIntegrationEventHandler<SchoolRemovedIntegrationEvent>
    {
        private readonly ISender _mediator;
        private readonly ILogger<SchoolRemovedIntegrationEventHandler> _logger;

        public SchoolRemovedIntegrationEventHandler(
            ISender mediator,
            ILogger<SchoolRemovedIntegrationEventHandler> logger)
        {
            _mediator = Guard.Against.Null(mediator, nameof(mediator));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }
        public async Task<Result> Handle(SchoolRemovedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{AppName}"))
            {
                _logger.LogInformation(
                    "----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                    @event.Id, AppName, @event);


                var command = new RemoveUsersWithGivenClaimCommand(
                    CustomClaimTypes.SchoolId, @event.SchoolId);

                var result = await _mediator.Send(
                    new IdentifiedCommand<RemoveUsersWithGivenClaimCommand>(command, @event.Id));

                return result;
            }
        }
    }
}