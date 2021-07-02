using System;
using MediatR;
using SharedKernel.Domain.Common;

namespace SharedKernel.Infrastructure.Concretes.Models
{
    public class DomainEventNotification<TDomainEvent> : INotification where TDomainEvent : DomainEvent
    {
        public DomainEventNotification(TDomainEvent domainEvent)
        {
            DomainEvent = domainEvent ?? throw new ArgumentNullException(nameof(domainEvent));
        }

        public TDomainEvent DomainEvent { get; }
    }
}