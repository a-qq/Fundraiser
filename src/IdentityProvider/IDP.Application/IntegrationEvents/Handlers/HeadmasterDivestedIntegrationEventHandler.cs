using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using IdentityModel;
using IDP.Application.Common.Models;
using IDP.Application.IntegrationEvents.Events;
using IDP.Application.Users.Commands.UpdateClaims;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using SharedKernel.Infrastructure.Abstractions.EventBus;
using System.Collections.Generic;
using System.Threading.Tasks;
using static IDP.Application.MediatorModule;

namespace IDP.Application.IntegrationEvents.Handlers
{
    internal sealed class HeadmasterDivestedIntegrationEventHandler : IIntegrationEventHandler<HeadmasterDivestedIntegrationEvent>
    {
        private readonly ISender _mediator;
        private readonly ILogger<HeadmasterDivestedIntegrationEventHandler> _logger;

        public HeadmasterDivestedIntegrationEventHandler(
            ISender mediator,
            ILogger<HeadmasterDivestedIntegrationEventHandler> logger)
        {
            _mediator = Guard.Against.Null(mediator, nameof(mediator));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        public async Task<Result> Handle(HeadmasterDivestedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{AppName}"))
            {
                _logger.LogInformation(
                    "----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                    @event.Id, AppName, @event);

                var claimsToUpdate = new List<ClaimUpdateSpecification>
                {
                    new ClaimUpdateSpecification(
                        JwtClaimTypes.Role, @event.AssignedRole, @event.RemovedRole),
                };

                var command = new UpdateClaimsCommand(@event.HeadmasterId, claimsToUpdate);

                var result = await _mediator.Send(
                    new IdentifiedCommand<UpdateClaimsCommand>(command, @event.Id));

                return result;
            }
        }
    }
}