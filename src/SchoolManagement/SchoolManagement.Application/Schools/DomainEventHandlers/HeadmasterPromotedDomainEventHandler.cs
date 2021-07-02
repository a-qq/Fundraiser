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
    internal sealed class HeadmasterPromotedDomainEventHandler
        : INotificationHandler<DomainEventNotification<HeadmasterPromotedDomainEvent>>
    {
        private readonly ILoggerFactory _logger;
        private readonly IIntegrationEventService _integrationEventService;

        public HeadmasterPromotedDomainEventHandler(ILoggerFactory logger,
            IIntegrationEventService integrationEventService)
        {
            _logger = logger;
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(DomainEventNotification<HeadmasterPromotedDomainEvent> notification,
            CancellationToken cancellationToken)
        {
            _logger.CreateLogger<HeadmasterPromotedDomainEvent>()
                .LogTrace("{Role} with Id: {TeacherId} has been successfully promoted to {Role}!",
                    SchoolRole.Teacher, notification.DomainEvent.TeacherId, SchoolRole.Headmaster);

            await _integrationEventService.AddAndSaveEventAsync(
                new HeadmasterPromotedIntegrationEvent(notification.DomainEvent.TeacherId, notification.DomainEvent.IsActive));
        }
    }
}