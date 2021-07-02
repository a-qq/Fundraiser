using MediatR;
using Microsoft.Extensions.Logging;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.IntegrationEvents.Events;
using SchoolManagement.Domain.SchoolAggregate.Groups;
using SchoolManagement.Domain.SchoolAggregate.Schools.Events;
using SharedKernel.Infrastructure.Concretes.Models;
using System.Threading;
using System.Threading.Tasks;
using SharedKernel.Domain.Constants;

namespace SchoolManagement.Application.Schools.DomainEventHandlers
{
    internal sealed class TreasurersDivestedDomainEventHandler
        : INotificationHandler<DomainEventNotification<TreasurersDivestedDomainEvent>>
    {

        private readonly ILoggerFactory _logger;
        private readonly IIntegrationEventService _integrationEventService;

        public TreasurersDivestedDomainEventHandler(ILoggerFactory logger,
            IIntegrationEventService integrationEventService)
        {
            _logger = logger;
            _integrationEventService = integrationEventService;
        }


        public async Task Handle(DomainEventNotification<TreasurersDivestedDomainEvent> notification,
            CancellationToken cancellationToken)
        {
            _logger.CreateLogger<TreasurerDivestedDomainEvent>()
                .LogTrace("{Role}s with Ids: {TreasurerId} has been successfully divested!",
                    GroupRoles.Treasurer, string.Join(", ", notification.DomainEvent.TreasurerData));

            await _integrationEventService.AddAndSaveEventAsync(
                new TreasurersDivestedIntegrationEvent(notification.DomainEvent.TreasurerData));
        }
    }
}