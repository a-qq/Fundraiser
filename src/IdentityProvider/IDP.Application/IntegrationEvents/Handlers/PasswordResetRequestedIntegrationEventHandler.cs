using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using IDP.Application.Common.Models;
using IDP.Application.IntegrationEvents.Events;
using IDP.Application.Users.Commands.SendResetPasswordEmail;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using SharedKernel.Infrastructure.Abstractions.EventBus;
using System.Threading.Tasks;
using static IDP.Application.MediatorModule;

namespace IDP.Application.IntegrationEvents.Handlers
{
    internal sealed class PasswordResetRequestedIntegrationEventHandler : IIntegrationEventHandler<PasswordResetRequestedIntegrationEvent>
    {
        private readonly ISender _mediator;
        private readonly ILogger<PasswordResetRequestedIntegrationEventHandler> _logger;

        public PasswordResetRequestedIntegrationEventHandler(
            ISender mediator,
            ILogger<PasswordResetRequestedIntegrationEventHandler> logger)
        {
            _mediator = Guard.Against.Null(mediator, nameof(mediator));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        public async Task<Result> Handle(PasswordResetRequestedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{AppName}"))
            {
                _logger.LogInformation(
                    "----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                    @event.Id, AppName, @event);

                var command = new SendResetPasswordEmailCommand(@event.Subject);

                var result = await _mediator.Send(new IdentifiedCommand<SendResetPasswordEmailCommand>(command, @event.Id));

                return result;
            }
        }
    }
}