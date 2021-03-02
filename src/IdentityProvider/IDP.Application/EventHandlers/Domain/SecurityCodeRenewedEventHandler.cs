using System.Threading;
using System.Threading.Tasks;
using IDP.Application.Common.Interfaces;
using IDP.Domain.UserAggregate.Events;
using MediatR;
using SharedKernel.Infrastructure.Implementations;

namespace IDP.Application.EventHandlers.Domain
{
    internal sealed class
        SecurityCodeRenewedEventHandler : INotificationHandler<DomainEventNotification<SecurityCodeRenewed>>
    {
        private readonly IIdpMailManager _mailManager;

        public SecurityCodeRenewedEventHandler(IIdpMailManager mailManager)
        {
            _mailManager = mailManager;
        }

        public async Task Handle(DomainEventNotification<SecurityCodeRenewed> notification,
            CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;
            await _mailManager.SendResetPasswordEmail(domainEvent.Email, domainEvent.SecurityCode);
        }
    }
}