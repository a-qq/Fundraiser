﻿using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using IdentityModel;
using IDP.Application.Common.Models;
using IDP.Application.IntegrationEvents.Events;
using IDP.Application.Users.Commands.RemoveClaimsFromUser;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using SharedKernel.Infrastructure.Abstractions.EventBus;
using System.Collections.Generic;
using System.Threading.Tasks;
using static IDP.Application.MediatorModule;

namespace IDP.Application.IntegrationEvents.Handlers
{
    internal sealed class TreasurerDivestedIntegrationEventHandler : IIntegrationEventHandler<TreasurerDivestedIntegrationEvent>
    {

        private readonly ISender _mediator;
        private readonly ILogger<TreasurerDivestedIntegrationEventHandler> _logger;

        public TreasurerDivestedIntegrationEventHandler(
            ISender mediator,
            ILogger<TreasurerDivestedIntegrationEventHandler> logger)
        {
            _mediator = Guard.Against.Null(mediator, nameof(mediator));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        public async Task<Result> Handle(TreasurerDivestedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{AppName}"))
            {
                _logger.LogInformation(
                    "----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                    @event.Id, AppName, @event);

                var claimsToRemove = new List<ClaimDeleteSpecification>
                {
                    new ClaimDeleteSpecification(JwtClaimTypes.Role, @event.RemovedRole)
                };

                var command = new RemoveClaimsFromUserCommand(@event.TreasurerId, claimsToRemove);

                var result = await _mediator.Send(
                    new IdentifiedCommand<RemoveClaimsFromUserCommand>(command, @event.Id));

                return result;
            }
        }
    }
}