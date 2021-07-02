using MediatR;
using Microsoft.Extensions.Logging;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.IntegrationEvents.Events;
using SchoolManagement.Domain.SchoolAggregate.Schools.Events;
using SharedKernel.Domain.Constants;
using SharedKernel.Infrastructure.Concretes.Models;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Schools.DomainEventHandlers
{
    internal sealed class HeadmasterDivestedDomainEventHandler 
        : INotificationHandler<DomainEventNotification<HeadmasterDivestedDomainEvent>>
    {
        private readonly ILoggerFactory _logger;
        private readonly IIntegrationEventService _integrationEventService;

        public HeadmasterDivestedDomainEventHandler(ILoggerFactory logger,
            IIntegrationEventService integrationEventService)
        {
            _logger = logger;
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(DomainEventNotification<HeadmasterDivestedDomainEvent> notification,
            CancellationToken cancellationToken)
        {
            _logger.CreateLogger<HeadmasterDivestedDomainEvent>()
                .LogTrace("{Role} with Id: {HeadmasterId} has been successfully divested!",
                    SchoolRole.Headmaster, notification.DomainEvent.HeadmasterId);

            await _integrationEventService.AddAndSaveEventAsync(
                new HeadmasterDivestedIntegrationEvent(notification.DomainEvent.HeadmasterId, notification.DomainEvent.IsActive));
        }
    }
}