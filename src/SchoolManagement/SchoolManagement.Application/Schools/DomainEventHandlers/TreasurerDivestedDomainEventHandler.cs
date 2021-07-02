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
    internal sealed class TreasurerDivestedDomainEventHandler 
        : INotificationHandler<DomainEventNotification<TreasurerDivestedDomainEvent>>
    {

        private readonly ILoggerFactory _logger;
        private readonly IIntegrationEventService _integrationEventService;

        public TreasurerDivestedDomainEventHandler(ILoggerFactory logger,
            IIntegrationEventService integrationEventService)
        {
            _logger = logger;
            _integrationEventService = integrationEventService;
        }


        public async Task Handle(DomainEventNotification<TreasurerDivestedDomainEvent> notification,
            CancellationToken cancellationToken)
        {
            _logger.CreateLogger<TreasurerDivestedDomainEvent>()
                .LogTrace("{Role} with Id: {TreasurerId} has been successfully divested!",
                    GroupRoles.Treasurer, notification.DomainEvent.TreasurerId);

            await _integrationEventService.AddAndSaveEventAsync(
                new TreasurerDivestedIntegrationEvent(notification.DomainEvent.TreasurerId, notification.DomainEvent.IsActive));
        }
    }
}