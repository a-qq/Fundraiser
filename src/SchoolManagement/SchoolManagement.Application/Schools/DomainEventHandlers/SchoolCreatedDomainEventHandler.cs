using MediatR;
using Microsoft.Extensions.Logging;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.IntegrationEvents.Events;
using SchoolManagement.Domain.SchoolAggregate.Schools.Events;
using SharedKernel.Infrastructure.Concretes.Models;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Schools.DomainEventHandlers
{
    internal sealed class SchoolCreatedDomainEventHandler : INotificationHandler<DomainEventNotification<SchoolCreatedDomainEvent>>
    {
        private readonly ILoggerFactory _logger;
        private readonly IIntegrationEventService _integrationEventService;

        public SchoolCreatedDomainEventHandler(ILoggerFactory logger,
            IIntegrationEventService integrationEventService)
        {
            _logger = logger;
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(DomainEventNotification<SchoolCreatedDomainEvent> notification,
            CancellationToken cancellationToken)
        {
            _logger.CreateLogger<SchoolCreatedDomainEvent>()
                .LogTrace("School with Id: {SchoolId} has been successfully created!", notification.DomainEvent.SchoolId);

            await _integrationEventService.AddAndSaveEventAsync(
                new SchoolCreatedIntegrationEvent(notification.DomainEvent.SchoolId));
        }
    }
}