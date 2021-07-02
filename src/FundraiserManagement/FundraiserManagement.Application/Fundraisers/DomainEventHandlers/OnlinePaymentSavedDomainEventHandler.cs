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
    internal sealed class OnlinePaymentSavedDomainEventHandler : INotificationHandler<DomainEventNotification<OnlinePaymentSavedDomainEvent>>
    {
        private readonly ILoggerFactory _logger;
        private readonly IIntegrationEventService _integrationEventService;

        public OnlinePaymentSavedDomainEventHandler(ILoggerFactory logger,
            IIntegrationEventService integrationEventService)
        {
            _logger = logger;
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(DomainEventNotification<OnlinePaymentSavedDomainEvent> notification,
            CancellationToken token)
        {
            _logger.CreateLogger<OnlinePaymentSavedDomainEvent>()
                .LogTrace("Online payment with Id: {PaymentId} has been successfully saved for further processing!",
                    notification.DomainEvent.PaymentId);

            await _integrationEventService.AddAndSaveEventAsync(
                new OnlinePaymentSavedApplicationEvent(notification.DomainEvent.PaymentId));
        }
    }
}