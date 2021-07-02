using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using IDP.Application.Common.Models;
using IDP.Application.IntegrationEvents.Events;
using IDP.Application.Users.Commands.RemoveUser;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using SharedKernel.Infrastructure.Abstractions.EventBus;
using System.Threading.Tasks;
using static IDP.Application.MediatorModule;

namespace IDP.Application.IntegrationEvents.Handlers
{
    internal sealed class MemberExpelledIntegrationEventHandler : IIntegrationEventHandler<MemberExpelledIntegrationEvent>
    {
        private readonly ISender _mediator;
        private readonly ILogger<MemberExpelledIntegrationEventHandler> _logger;

        public MemberExpelledIntegrationEventHandler(
            ISender mediator,
            ILogger<MemberExpelledIntegrationEventHandler> logger)
        {
            _mediator = Guard.Against.Null(mediator, nameof(mediator));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        public async Task<Result> Handle(MemberExpelledIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{AppName}"))
            {
                _logger.LogInformation(
                    "----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                    @event.Id, AppName, @event);


                var command = new RemoveUserCommand(@event.MemberId);

                var result = await _mediator.Send(
                    new IdentifiedCommand<RemoveUserCommand>(command, @event.Id));

                return result;
            }
        }
    }
}