using IDP.Domain.UserAggregate.ValueObjects;
using SharedKernel.Domain.Common;
using SharedKernel.Domain.ValueObjects;
using System;

namespace IDP.Domain.UserAggregate.Events
{
    public sealed class SecurityCodeRenewed : DomainEvent
    {
        public Email Email { get; }
        public SecurityCode SecurityCode { get; }

        internal SecurityCodeRenewed(Email email, SecurityCode securityCode)
        {
            Email = email ?? throw new ArgumentNullException(nameof(email));
            SecurityCode = securityCode ?? throw new ArgumentNullException(nameof(securityCode));
        }
    }
}
