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
    internal sealed class MemberArchivedDomainEventHandler
        : INotificationHandler<DomainEventNotification<MemberArchivedDomainEvent>>
    {
        private readonly ILoggerFactory _logger;
        private readonly IIntegrationEventService _integrationEventService;

        public MemberArchivedDomainEventHandler(ILoggerFactory logger,
            IIntegrationEventService integrationEventService)
        {
            _logger = logger;
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(DomainEventNotification<MemberArchivedDomainEvent> notification,
            CancellationToken cancellationToken)
        {
            _logger.CreateLogger<MemberArchivedDomainEvent>()
                .LogTrace("Member with Id: {MemberId} has been successfully archived!",
                    notification.DomainEvent.MemberId);

            await _integrationEventService.AddAndSaveEventAsync(
                new MemberArchivedIntegrationEvent(notification.DomainEvent.MemberId, notification.DomainEvent.GroupRole));
        }
    }
}