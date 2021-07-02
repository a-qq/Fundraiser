using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using IdentityModel;
using IDP.Application.Common.Models;
using IDP.Application.IntegrationEvents.Events;
using IDP.Application.Users.Commands.DeactivateUser;
using IDP.Application.Users.Commands.DeactivateUsers;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using SharedKernel.Domain.Utils;
using SharedKernel.Infrastructure.Abstractions.EventBus;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static IDP.Application.MediatorModule;

namespace IDP.Application.IntegrationEvents.Handlers
{
    internal sealed class MembersArchivedIntegrationEventHandler : IIntegrationEventHandler<MembersArchivedIntegrationEvent>
    {
        private readonly ISender _mediator;
        private readonly ILogger<MembersArchivedIntegrationEventHandler> _logger;

        public MembersArchivedIntegrationEventHandler(
            ISender mediator,
            ILogger<MembersArchivedIntegrationEventHandler> logger)
        {
            _mediator = Guard.Against.Null(mediator, nameof(mediator));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        public async Task<Result> Handle(MembersArchivedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{AppName}"))
            {
                _logger.LogInformation(
                    "----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                    @event.Id, AppName, @event);


                var command = new DeactivateUsersCommand(@event.MembersData.Select(e =>
                {
                    var claimsDeleteSpecifications = new List<ClaimDeleteSpecification>
                    {
                        new ClaimDeleteSpecification(CustomClaimTypes.GroupId)
                    };

                    if (!(e.GroupRole is null))
                        claimsDeleteSpecifications.Add(new ClaimDeleteSpecification(JwtClaimTypes.Role, e.GroupRole));

                    return new DeactivateUserCommand(e.MemberId, claimsDeleteSpecifications);
                }));

                var result = await _mediator.Send(
                    new IdentifiedCommand<DeactivateUsersCommand>(command, @event.Id));

                return result;
            }
        }
    }
}