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
    internal sealed class TreasurerPromotedDomainEventHandler
        : INotificationHandler<DomainEventNotification<TreasurerPromotedDomainEvent>>
    {
        private readonly ILoggerFactory _logger;
        private readonly IIntegrationEventService _integrationEventService;

        public TreasurerPromotedDomainEventHandler(ILoggerFactory logger,
            IIntegrationEventService integrationEventService)
        {
            _logger = logger;
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(DomainEventNotification<TreasurerPromotedDomainEvent> notification,
            CancellationToken cancellationToken)
        {
            _logger.CreateLogger<TreasurerDivestedDomainEvent>()
                .LogTrace("{Role} with Id: {studentId} has been successfully promoted to {role}!",
                    SchoolRole.Student, notification.DomainEvent.StudentId, GroupRoles.Treasurer);

            await _integrationEventService.AddAndSaveEventAsync(
                new TreasurerPromotedIntegrationEvent(notification.DomainEvent.StudentId, notification.DomainEvent.IsActive));
        }
    }
}