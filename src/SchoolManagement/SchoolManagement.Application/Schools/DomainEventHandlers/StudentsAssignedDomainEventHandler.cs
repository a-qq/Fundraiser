using System.Linq;
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
    internal sealed class StudentsAssignedDomainEventHandler 
        : INotificationHandler<DomainEventNotification<StudentsAssignedDomainEvent>>
    {
        private readonly ILoggerFactory _logger;
        private readonly IIntegrationEventService _integrationEventService;

        public StudentsAssignedDomainEventHandler(ILoggerFactory logger,
            IIntegrationEventService integrationEventService)
        {
            _logger = logger;
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(DomainEventNotification<StudentsAssignedDomainEvent> notification,
            CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;
            _logger.CreateLogger<MemberArchivedDomainEvent>()
                .LogTrace("{role}s with Ids: {studentIds} has been successfully assigned to group with Id: {groupId}!",
                    SchoolRole.Student, string.Join(", ", domainEvent.StudentData.Select(d => d.MemberId)), domainEvent.GroupId);

            await _integrationEventService.AddAndSaveEventAsync(
                new StudentsAssignedIntegrationEvent(domainEvent.GroupId, domainEvent.StudentData));
        }
    }
}