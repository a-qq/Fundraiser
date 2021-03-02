﻿using MediatR;
using SharedKernel.Domain.Common;
using System;

namespace SharedKernel.Infrastructure.Implementations
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
