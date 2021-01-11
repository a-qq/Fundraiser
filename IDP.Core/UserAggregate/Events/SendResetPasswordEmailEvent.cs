using Fundraiser.SharedKernel.Utils;
using IDP.Core.UserAggregate.ValueObjects;
using MediatR;
using System;

namespace IDP.Core.UserAggregate.Events
{
    public sealed class SendResetPasswordEmailEvent : INotification
    {
        public Email Email { get; }
        public SecurityCode SecurityCode { get; }

        public SendResetPasswordEmailEvent(Email email, SecurityCode securityCode)
        {
            Email = email ?? throw new ArgumentNullException(nameof(email));
            SecurityCode = securityCode ?? throw new ArgumentNullException(nameof(securityCode));
        }
    }
}
