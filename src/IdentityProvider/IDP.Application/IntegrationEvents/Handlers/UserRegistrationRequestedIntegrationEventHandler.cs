using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using IDP.Application.IntegrationEvents.Events;
using IDP.Application.Users.Commands.SendCompleteRegistrationEmail;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using SharedKernel.Infrastructure.Abstractions.EventBus;
using System.Threading.Tasks;
using IDP.Application.Common.Models;
using static IDP.Application.MediatorModule;

namespace IDP.Application.IntegrationEvents.Handlers
{
    internal sealed class UserRegistrationRequestedIntegrationEventHandler : IIntegrationEventHandler<UserRegistrationRequestedIntegrationEvent>
    {
        private readonly ISender _mediator;
        private readonly ILogger<UserRegistrationRequestedIntegrationEventHandler> _logger;

        public UserRegistrationRequestedIntegrationEventHandler(
            ISender mediator,
            ILogger<UserRegistrationRequestedIntegrationEventHandler> logger)
        {
            _mediator = Guard.Against.Null(mediator, nameof(mediator));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        public async Task<Result> Handle(UserRegistrationRequestedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{AppName}"))
            {
                _logger.LogInformation(
                    "----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                    @event.Id, AppName, @event);

                var command = new SendCompleteRegistrationEmailCommand(@event.Subject);

                var result = await _mediator.Send(new IdentifiedCommand<SendCompleteRegistrationEmailCommand>(command, @event.Id));

                return result;
            }
        }
    }
}