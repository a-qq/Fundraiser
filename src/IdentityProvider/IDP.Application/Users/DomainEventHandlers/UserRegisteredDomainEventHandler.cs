using Ardalis.GuardClauses;
using IDP.Application.IntegrationEvents.Events;
using IDP.Domain.UserAggregate.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Infrastructure.Abstractions.Common;
using SharedKernel.Infrastructure.Concretes.Models;
using System.Threading;
using System.Threading.Tasks;

namespace IDP.Application.Users.DomainEventHandlers
{
    internal sealed class
        UserRegisteredDomainEventHandler : INotificationHandler<DomainEventNotification<UserRegisteredDomainEvent>>
    {
        private readonly ILoggerFactory _logger;
        private readonly IIntegrationEventService _integrationEventService;

        public UserRegisteredDomainEventHandler(
            ILoggerFactory logger,
            IIntegrationEventService integrationEventService)
        {
            _logger = Guard.Against.Null(logger, nameof(logger));
            _integrationEventService = Guard.Against.Null(integrationEventService, nameof(integrationEventService));
        }

        public async Task Handle(DomainEventNotification<UserRegisteredDomainEvent> notification,
            CancellationToken cancellationToken)
        {
            _logger.CreateLogger<UserRegisteredDomainEventHandler>()
                .LogTrace("User with subject '{Subject}' has successfully completed registration!",
                    notification.DomainEvent.Subject);

            await _integrationEventService.AddAndSaveEventAsync(
                new UserRegisteredIntegrationEvent(notification.DomainEvent.Subject));
        }
    }
}