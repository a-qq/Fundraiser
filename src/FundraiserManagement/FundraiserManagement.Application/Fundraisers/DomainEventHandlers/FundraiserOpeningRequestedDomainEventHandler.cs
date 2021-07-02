using FundraiserManagement.Application.Common.Interfaces.Services;
using FundraiserManagement.Application.IntegrationEvents.Local;
using FundraiserManagement.Domain.FundraiserAggregate.Fundraisers.DomainEvents;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Infrastructure.Concretes.Models;
using System.Threading;
using System.Threading.Tasks;

namespace FundraiserManagement.Application.Fundraisers.DomainEventHandlers
{
    internal sealed class FundraiserOpeningRequestedDomainEventHandler : INotificationHandler<DomainEventNotification<FundraiserOpeningRequestedDomainEvent>>
    {
        private readonly ILoggerFactory _logger;
        private readonly IIntegrationEventService _integrationEventService;

        public FundraiserOpeningRequestedDomainEventHandler(ILoggerFactory logger,
            IIntegrationEventService integrationEventService)
        {
            _logger = logger;
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(DomainEventNotification<FundraiserOpeningRequestedDomainEvent> notification,
            CancellationToken token)
        {
            _logger.CreateLogger<FundraiserOpeningRequestedDomainEvent>()
                .LogTrace("Fundraiser with Id: {PaymentId} has been successfully requested for opening!",
                    notification.DomainEvent.FundraiserId);

            await _integrationEventService.AddAndSaveEventAsync(new FundraiserOpeningRequestedApplicationEvent(
                notification.DomainEvent.FundraiserId, notification.DomainEvent.SchoolId));
        }
    }
}