using System;
using RabbitMQ.Client;


namespace SharedKernel.Infrastructure.Abstractions.EventBus.RabbitMQ
{
    public interface IRabbitMqPersistentConnection
        : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();

        IModel CreateModel();
    }
}
