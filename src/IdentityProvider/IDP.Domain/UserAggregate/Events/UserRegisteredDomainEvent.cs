﻿using Ardalis.GuardClauses;
using IDP.Domain.UserAggregate.ValueObjects;
using SharedKernel.Domain.Common;

namespace IDP.Domain.UserAggregate.Events
{
    public sealed class UserRegisteredDomainEvent : DomainEvent
    {
        public UserRegisteredDomainEvent(Subject subject)
        {
            Subject = Guard.Against.Null(subject, nameof(subject));
        }

        public string Subject { get; }
    }
}