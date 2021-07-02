using FundraiserManagement.Application.Fundraisers.Commands.SuspendFundraisers;
using FundraiserManagement.Domain.MemberAggregate.DomainEvents;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Infrastructure.Concretes.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FundraiserManagement.Application.Members.DomainEventHandlers
{
    internal sealed class MemberPermissionsDowngradedDomainEventHandler : INotificationHandler<DomainEventNotification<MemberPermissionsDowngradedDomainEvent>>
    {
        private readonly ILoggerFactory _logger;
        private readonly ISender _mediator;

        public MemberPermissionsDowngradedDomainEventHandler(
            ILoggerFactory logger,
            ISender mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public async Task Handle(DomainEventNotification<MemberPermissionsDowngradedDomainEvent> notification,
            CancellationToken token)
        {
            var logger = _logger.CreateLogger<MemberPermissionsDowngradedDomainEvent>();

            logger.LogTrace("Member with Id: {MemberId} has been successfully archived!", notification.DomainEvent.MemberId);

            var result = await _mediator.Send(new SuspendFundraisersCommand(notification.DomainEvent.MemberId,
                notification.DomainEvent.SchoolId, notification.DomainEvent.WasHeadmaster), token);

            if (result.IsFailure)
                throw new InvalidOperationException(result.Error);
        }
    }
}
