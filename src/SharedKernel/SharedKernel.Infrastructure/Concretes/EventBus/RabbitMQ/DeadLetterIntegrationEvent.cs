using System;
using SharedKernel.Infrastructure.Concretes.Models;

namespace SharedKernel.Infrastructure.Concretes.EventBus.RabbitMQ
{
    public sealed class DeadLetterIntegrationEvent : IntegrationEvent
    {
        public string Type { get; }
        public string Content { get; }
        public string Reason { get; }

        public DeadLetterIntegrationEvent(Guid id, DateTime creationDate, string type, string content, string reason)
            : base(id, creationDate)
        {
            Type = type;
            Content = content;
            Reason = reason;
        }
    }
}
