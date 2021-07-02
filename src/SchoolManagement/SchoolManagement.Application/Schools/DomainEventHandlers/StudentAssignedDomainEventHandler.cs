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
    internal sealed class StudentAssignedDomainEventHandler
        : INotificationHandler<DomainEventNotification<StudentAssignedDomainEvent>>
    {
        private readonly ILoggerFactory _logger;
        private readonly IIntegrationEventService _integrationEventService;

        public StudentAssignedDomainEventHandler(ILoggerFactory logger,
            IIntegrationEventService integrationEventService)
        {
            _logger = logger;
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(DomainEventNotification<StudentAssignedDomainEvent> notification,
            CancellationToken cancellationToken)
        {
            _logger.CreateLogger<StudentAssignedDomainEvent>()
                .LogTrace("{Role}s with Id: {StudentId} has been successfully assigned to group with Id: {GroupId}!",
                    SchoolRole.Student, notification.DomainEvent.StudentId, notification.DomainEvent.GroupId);

            await _integrationEventService.AddAndSaveEventAsync(new StudentAssignedIntegrationEvent(
                notification.DomainEvent.StudentId, notification.DomainEvent.GroupId, notification.DomainEvent.IsActive));
        }
    }
}