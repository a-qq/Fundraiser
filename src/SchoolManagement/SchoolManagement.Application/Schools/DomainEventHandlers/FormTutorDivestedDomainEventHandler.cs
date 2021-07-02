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
    internal sealed class FormTutorDivestedDomainEventHandler 
        : INotificationHandler<DomainEventNotification<FormTutorDivestedDomainEvent>>
    {
        private readonly ILoggerFactory _logger;
        private readonly IIntegrationEventService _integrationEventService;

        public FormTutorDivestedDomainEventHandler(ILoggerFactory logger,
            IIntegrationEventService integrationEventService)
        {
            _logger = logger;
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(DomainEventNotification<FormTutorDivestedDomainEvent> notification,
            CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;

            _logger.CreateLogger<FormTutorDivestedDomainEvent>()
                .LogTrace("{PreviousRole} with Id: {FormTutorId} has been successfully divested!",
                    GroupRoles.FormTutor, domainEvent.FormTutorId);

            await _integrationEventService.AddAndSaveEventAsync(
                new FormTutorDivestedIntegrationEvent(domainEvent.FormTutorId, domainEvent.IsActive));
        }
    }
}