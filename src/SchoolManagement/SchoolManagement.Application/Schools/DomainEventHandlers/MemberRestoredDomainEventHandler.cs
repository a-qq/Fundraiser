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
    internal sealed class MemberRestoredDomainEventHandler 
        : INotificationHandler<DomainEventNotification<MemberRestoredDomainEvent>>
    {
        private readonly ILoggerFactory _logger;
        private readonly IIntegrationEventService _integrationEventService;

        public MemberRestoredDomainEventHandler(ILoggerFactory logger,
            IIntegrationEventService integrationEventService)
        {
            _logger = logger;
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(DomainEventNotification<MemberRestoredDomainEvent> notification,
            CancellationToken cancellationToken)
        {
            _logger.CreateLogger<MemberArchivedDomainEvent>()
                .LogTrace("Member with Id: {MemberId} has been successfully restored from archived status!",
                    notification.DomainEvent.MemberId);

            await _integrationEventService.AddAndSaveEventAsync(
                new MemberRestoredIntegrationEvent(notification.DomainEvent.MemberId));
        }
    }
}