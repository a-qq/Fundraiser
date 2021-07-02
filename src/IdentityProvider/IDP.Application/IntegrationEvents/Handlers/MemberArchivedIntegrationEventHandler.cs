using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using IdentityModel;
using IDP.Application.Common.Models;
using IDP.Application.IntegrationEvents.Events;
using IDP.Application.Users.Commands.DeactivateUser;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using SharedKernel.Domain.Utils;
using SharedKernel.Infrastructure.Abstractions.EventBus;
using System.Collections.Generic;
using System.Threading.Tasks;
using static IDP.Application.MediatorModule;

namespace IDP.Application.IntegrationEvents.Handlers
{
    internal sealed class MemberArchivedIntegrationEventHandler : IIntegrationEventHandler<MemberArchivedIntegrationEvent>
    {
        private readonly ISender _mediator;
        private readonly ILogger<MemberArchivedIntegrationEventHandler> _logger;

        public MemberArchivedIntegrationEventHandler(
            ISender mediator,
            ILogger<MemberArchivedIntegrationEventHandler> logger)
        {
            _mediator = Guard.Against.Null(mediator, nameof(mediator));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        public async Task<Result> Handle(MemberArchivedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{AppName}"))
            {
                _logger.LogInformation(
                    "----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                    @event.Id, AppName, @event);

                var claimDeleteSpecifications = new List<ClaimDeleteSpecification>
                {
                    new ClaimDeleteSpecification(CustomClaimTypes.GroupId)
                };

                if (!(@event.GroupRole is null))
                    claimDeleteSpecifications.Add(new ClaimDeleteSpecification(JwtClaimTypes.Role, @event.GroupRole));

                var command = new DeactivateUserCommand(@event.MemberId, claimDeleteSpecifications);

                var result = await _mediator.Send(
                    new IdentifiedCommand<DeactivateUserCommand>(command, @event.Id));

                return result;
            }
        }
    }
}