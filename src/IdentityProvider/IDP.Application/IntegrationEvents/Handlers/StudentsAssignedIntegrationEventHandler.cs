using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using IDP.Application.Common.Models;
using IDP.Application.IntegrationEvents.Events;
using IDP.Application.Users.Commands.AddClaimsToUser;
using IDP.Application.Users.Commands.AddClaimsToUsers;
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
    internal sealed class StudentsAssignedIntegrationEventHandler : IIntegrationEventHandler<StudentsAssignedIntegrationEvent>
    {

        private readonly ISender _mediator;
        private readonly ILogger<StudentsAssignedIntegrationEvent> _logger;

        public StudentsAssignedIntegrationEventHandler(
            ISender mediator,
            ILogger<StudentsAssignedIntegrationEvent> logger)
        {
            _mediator = Guard.Against.Null(mediator, nameof(mediator));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        public async Task<Result> Handle(StudentsAssignedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{AppName}"))
            {
                _logger.LogInformation(
                    "----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                    @event.Id, AppName, @event);

                var claimsToAdd = new List<ClaimInsertModel>
                {
                    new ClaimInsertModel(CustomClaimTypes.GroupId, @event.GroupId)
                };

                var command = new AddClaimsToUsersCommand(
                    @event.MembersData.Select(id => new AddClaimsToUserCommand(id.MemberId, claimsToAdd)).ToList());

                var result = await _mediator.Send(
                    new IdentifiedCommand<AddClaimsToUsersCommand>(command, @event.Id));

                return result;
            }
        }
    }
}