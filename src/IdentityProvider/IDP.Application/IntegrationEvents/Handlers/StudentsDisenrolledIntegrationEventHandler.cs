using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using IdentityModel;
using IDP.Application.Common.Models;
using IDP.Application.IntegrationEvents.Events;
using IDP.Application.Users.Commands.RemoveClaimsFromUser;
using IDP.Application.Users.Commands.RemoveClaimsFromUsers;
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
    internal sealed class StudentsDisenrolledIntegrationEventHandler : IIntegrationEventHandler<StudentsDisenrolledIntegrationEvent>
    {

        private readonly ISender _mediator;
        private readonly ILogger<StudentsDisenrolledIntegrationEvent> _logger;

        public StudentsDisenrolledIntegrationEventHandler(
            ISender mediator,
            ILogger<StudentsDisenrolledIntegrationEvent> logger)
        {
            _mediator = Guard.Against.Null(mediator, nameof(mediator));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        public async Task<Result> Handle(StudentsDisenrolledIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{AppName}"))
            {
                _logger.LogInformation(
                    "----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                    @event.Id, AppName, @event);

                var groupDeleteSpec = new ClaimDeleteSpecification(CustomClaimTypes.GroupId);

                var command = new RemoveClaimsFromUsersCommand(
                    @event.DisenrolledStudentsData.Select(e =>
                    {
                        var claimsToRemove = new List<ClaimDeleteSpecification>
                        {
                            groupDeleteSpec
                        };

                        if (!string.IsNullOrWhiteSpace(e.RemovedRole))
                            claimsToRemove.Add(new ClaimDeleteSpecification(JwtClaimTypes.Role, e.RemovedRole));

                        return new RemoveClaimsFromUserCommand(e.StudentId, claimsToRemove);
                    }).ToList());

                var result = await _mediator.Send(
                    new IdentifiedCommand<RemoveClaimsFromUsersCommand>(command, @event.Id));

                return result;
            }
        }
    }
}