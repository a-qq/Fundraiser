using IDP.Application.Common.Interfaces;
using IDP.Domain.UserAggregate.Events;
using MediatR;
using SharedKernel.Infrastructure.Implementations;
using System.Threading;
using System.Threading.Tasks;

namespace IDP.Infrastructure.EventHandlers.Domain
{
    internal sealed class SecurityCodeRenewedEventHandler : INotificationHandler<DomainEventNotification<SecurityCodeRenewed>>
    {
        private readonly IIdpMailManager _mailManager;

        public SecurityCodeRenewedEventHandler(IIdpMailManager mailManager)
        {
            _mailManager = mailManager;
        }

        public async Task Handle(DomainEventNotification<SecurityCodeRenewed> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;
            await _mailManager.SendResetPasswordEmail(domainEvent.Email, domainEvent.SecurityCode);
        }
    }
}
