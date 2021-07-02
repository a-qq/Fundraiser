using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using IDP.Application.Common.Models;
using IDP.Application.IntegrationEvents.Events;
using IDP.Application.Users.Commands.ReactivateUser;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using SharedKernel.Infrastructure.Abstractions.EventBus;
using System.Threading.Tasks;
using static IDP.Application.MediatorModule;

namespace IDP.Application.IntegrationEvents.Handlers
{
    internal sealed class MemberRestoredIntegrationEventHandler : IIntegrationEventHandler<MemberRestoredIntegrationEvent>
    {
        private readonly ISender _mediator;
        private readonly ILogger<MemberRestoredIntegrationEventHandler> _logger;

        public MemberRestoredIntegrationEventHandler(
            ISender mediator,
            ILogger<MemberRestoredIntegrationEventHandler> logger)
        {
            _mediator = Guard.Against.Null(mediator, nameof(mediator));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        public async Task<Result> Handle(MemberRestoredIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{AppName}"))
            {
                _logger.LogInformation(
                    "----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                    @event.Id, AppName, @event);


                var command = new ReactivateUserCommand(@event.MemberId);

                var result = await _mediator.Send(
                    new IdentifiedCommand<ReactivateUserCommand>(command, @event.Id));

                return result;
            }
        }
    }
}