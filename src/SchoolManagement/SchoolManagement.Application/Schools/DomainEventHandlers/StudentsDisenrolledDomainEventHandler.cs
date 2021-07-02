using MediatR;
using Microsoft.Extensions.Logging;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.IntegrationEvents.Events;
using SchoolManagement.Domain.SchoolAggregate.Schools.Events;
using SharedKernel.Domain.Constants;
using SharedKernel.Infrastructure.Concretes.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Schools.DomainEventHandlers
{
    internal sealed class StudentsDisenrolledDomainEventHandler
        : INotificationHandler<DomainEventNotification<StudentsDisenrolledDomainEvent>>
    {
        private readonly ILoggerFactory _logger;
        private readonly IIntegrationEventService _integrationEventService;

        public StudentsDisenrolledDomainEventHandler(ILoggerFactory logger,
            IIntegrationEventService integrationEventService)
        {
            _logger = logger;
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(DomainEventNotification<StudentsDisenrolledDomainEvent> notification, CancellationToken cancellationToken)
        {
            _logger.CreateLogger<StudentsDisenrolledDomainEvent>()
                .LogTrace("{Student}s with Ids: {StudentId} has been successfully removed from group!",
                    SchoolRole.Student, string.Join(", ", notification.DomainEvent.DisenrolledStudentsData.Select(d => d.MemberId)));

            await _integrationEventService.AddAndSaveEventAsync(
                new StudentsDisenrolledIntegrationEvent(notification.DomainEvent.DisenrolledStudentsData));
        }
    }
}