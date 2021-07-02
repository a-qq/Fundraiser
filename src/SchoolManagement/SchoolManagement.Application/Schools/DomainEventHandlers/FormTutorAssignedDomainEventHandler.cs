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
    internal sealed class FormTutorAssignedDomainEventHandler 
        : INotificationHandler<DomainEventNotification<FormTutorAssignedDomainEvent>>
    {
        private readonly ILoggerFactory _logger;
        private readonly IIntegrationEventService _integrationEventService;

        public FormTutorAssignedDomainEventHandler(ILoggerFactory logger,
            IIntegrationEventService integrationEventService)
        {
            _logger = logger;
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(DomainEventNotification<FormTutorAssignedDomainEvent> notification,
            CancellationToken cancellationToken)
        {
            _logger.CreateLogger<FormTutorAssignedDomainEvent>()
                .LogTrace("{PreviousRole} with Id: {TeacherId} has been successfully promoted to {AnotherRole} of group with Id: {GroupId})!",
                    SchoolRole.Teacher, notification.DomainEvent.TeacherId, GroupRoles.FormTutor, notification.DomainEvent.GroupId);

            await _integrationEventService.AddAndSaveEventAsync(
                new FormTutorAssignedIntegrationEvent(notification.DomainEvent.TeacherId, notification.DomainEvent.GroupId, notification.DomainEvent.IsActive));
        }
    }
}