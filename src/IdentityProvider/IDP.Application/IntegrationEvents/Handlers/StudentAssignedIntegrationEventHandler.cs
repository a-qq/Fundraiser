using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using IDP.Application.Common.Models;
using IDP.Application.IntegrationEvents.Events;
using IDP.Application.Users.Commands.AddClaimsToUser;
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
    internal sealed class StudentAssignedIntegrationEventHandler
        : IIntegrationEventHandler<StudentAssignedIntegrationEvent>
    {
        private readonly ISender _mediator;
        private readonly ILogger<StudentAssignedIntegrationEventHandler> _logger;

        public StudentAssignedIntegrationEventHandler(
            ISender mediator,
            ILogger<StudentAssignedIntegrationEventHandler> logger)
        {
            _mediator = Guard.Against.Null(mediator, nameof(mediator));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        public async Task<Result> Handle(StudentAssignedIntegrationEvent @event)
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

                var command = new AddClaimsToUserCommand(@event.StudentId, claimsToAdd);

                var result = await _mediator.Send(new IdentifiedCommand<AddClaimsToUserCommand>(command, @event.Id));

                return result;
            }
        }
    }
}