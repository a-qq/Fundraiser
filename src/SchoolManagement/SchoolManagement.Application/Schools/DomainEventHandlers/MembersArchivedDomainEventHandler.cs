using MediatR;
using Microsoft.Extensions.Logging;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.IntegrationEvents.Events;
using SchoolManagement.Domain.SchoolAggregate.Schools.Events;
using SharedKernel.Infrastructure.Concretes.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Schools.DomainEventHandlers
{
    public sealed class MembersArchivedDomainEventHandler
        : INotificationHandler<DomainEventNotification<MembersArchivedDomainEvent>>
    {
        private readonly ILoggerFactory _logger;
        private readonly IIntegrationEventService _integrationEventService;

        public MembersArchivedDomainEventHandler(ILoggerFactory logger,
            IIntegrationEventService integrationEventService)
        {
            _logger = logger;
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(DomainEventNotification<MembersArchivedDomainEvent> notification,
            CancellationToken cancellationToken)
        {
            _logger.CreateLogger<MembersArchivedDomainEvent>()
                .LogTrace("Members with Ids: {MemberIds} has been successfully archived!",
                    string.Join(", ", notification.DomainEvent.MembersData.Select(d => d.MemberId)));

            await _integrationEventService.AddAndSaveEventAsync(
                new MembersArchivedIntegrationEvent(notification.DomainEvent.MembersData));
        }
    }
}