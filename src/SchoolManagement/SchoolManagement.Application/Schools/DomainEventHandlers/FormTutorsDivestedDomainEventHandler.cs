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
    public sealed class FormTutorsDivestedDomainEventHandler : INotificationHandler<DomainEventNotification<FormTutorsDivestedDomainEvent>>
    {
        private readonly ILoggerFactory _logger;
        private readonly IIntegrationEventService _integrationEventService;

        public FormTutorsDivestedDomainEventHandler(
            ILoggerFactory logger,
            IIntegrationEventService integrationEventService)
        {
            _logger = logger;
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(DomainEventNotification<FormTutorsDivestedDomainEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;

            _logger.CreateLogger<FormTutorsDivestedDomainEvent>()
                .LogTrace("{PreviousRole}s with Ids: {FormTutorsData} has been successfully divested!",
                    GroupRoles.FormTutor, string.Join(", ", domainEvent.FormTutorsData));

            await _integrationEventService.AddAndSaveEventAsync(
                new FormTutorsDivestedIntegrationEvent(domainEvent.FormTutorsData));
        }
    }
}
