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
    internal sealed class PasswordResetRequestedDomainEventHandler : INotificationHandler<DomainEventNotification<PasswordResetRequestedDomainEvent>>
    {
        private readonly ILoggerFactory _logger;
        private readonly IIntegrationEventService _integrationEventService;

        public PasswordResetRequestedDomainEventHandler(
            ILoggerFactory logger,
            IIntegrationEventService integrationEventService)
        {
            _logger = Guard.Against.Null(logger, nameof(logger));
            _integrationEventService = Guard.Against.Null(integrationEventService, nameof(integrationEventService));
        }

        public async Task Handle(DomainEventNotification<PasswordResetRequestedDomainEvent> notification,
            CancellationToken cancellationToken)
        {
            _logger.CreateLogger<PasswordResetRequestedDomainEvent>()
                .LogTrace("User with subject {Subject} has successfully requested password reset!",
                    notification.DomainEvent.Subject);

            await _integrationEventService.AddAndSaveEventAsync(
                new PasswordResetRequestedIntegrationEvent(notification.DomainEvent.Subject));
        }
    }
}