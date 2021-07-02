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
    internal sealed class StudentDisenrolledDomainEventHandler
        : INotificationHandler<DomainEventNotification<StudentDisenrolledDomainEvent>>
    {
        private readonly ILoggerFactory _logger;
        private readonly IIntegrationEventService _integrationEventService;

        public StudentDisenrolledDomainEventHandler(ILoggerFactory logger,
            IIntegrationEventService integrationEventService)
        {
            _logger = logger;
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(DomainEventNotification<StudentDisenrolledDomainEvent> notification, CancellationToken cancellationToken)
        {
            _logger.CreateLogger<StudentDisenrolledDomainEvent>()
                .LogTrace("{Student} with Id: {StudentId} has been successfully removed from group!",
                    SchoolRole.Student, notification.DomainEvent.StudentId);

            await _integrationEventService.AddAndSaveEventAsync(
                new StudentDisenrolledIntegrationEvent(notification.DomainEvent.StudentId,
                    notification.DomainEvent.RemovedRole, notification.DomainEvent.IsActive));
        }
    }
}
