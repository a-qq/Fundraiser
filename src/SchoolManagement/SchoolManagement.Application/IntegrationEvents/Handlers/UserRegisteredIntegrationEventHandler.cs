using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.Extensions.Logging;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.IntegrationEvents.Events;
using SchoolManagement.Application.Schools.Commands.MarkMemberAsActive;
using Serilog.Context;
using SharedKernel.Infrastructure.Abstractions.EventBus;
using System.Threading.Tasks;
using static SchoolManagement.Application.ApplicationModule;

namespace SchoolManagement.Application.IntegrationEvents.Handlers
{
    internal sealed class UserRegisteredIntegrationEventHandler : IIntegrationEventHandler<UserRegisteredIntegrationEvent>
    {
        private readonly ISender _mediator;
        private readonly ILogger<UserRegisteredIntegrationEventHandler> _logger;

        public UserRegisteredIntegrationEventHandler(
            ISender mediator,
            ILogger<UserRegisteredIntegrationEventHandler> logger)
        {
            _mediator = Guard.Against.Null(mediator, nameof(mediator));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        public async Task<Result> Handle(UserRegisteredIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{AppName}"))
            {
                _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, AppName, @event);

                var command = new MarkMemberAsActiveCommand(@event.Subject);

                var result = await _mediator.Send(new IdentifiedCommand<MarkMemberAsActiveCommand>(command, @event.Id));

                return result;
            }
        }
    }
}
