using Autofac;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using SharedKernel.Infrastructure.Abstractions.EventBus;
using SharedKernel.Infrastructure.Abstractions.EventBus.RabbitMQ;
using SharedKernel.Infrastructure.Abstractions.IntegrationEventLogEF;
using SharedKernel.Infrastructure.Concretes.EventBus;
using SharedKernel.Infrastructure.Concretes.EventBus.RabbitMQ;
using SharedKernel.Infrastructure.Concretes.IntegrationEventLogEF;

namespace SharedKernel.Infrastructure
{
    public sealed class RabbitMqEventBusModule : Autofac.Module
    {
        private readonly string _subscriptionClientName;
        private readonly int _retryCount;
        private readonly string _eventBusConnection;
        private readonly string _eventBusUserName;
        private readonly string _eventBusPassword;

        public RabbitMqEventBusModule(
            string subscriptionClientName,
            int retryCount,
            string eventBusConnection,
            string eventBusUserName,
            string eventBusPassword)
        {
            _subscriptionClientName = subscriptionClientName;
            _retryCount = retryCount < 0 ? 5 : retryCount;
            _eventBusConnection = eventBusConnection;
            _eventBusUserName = eventBusUserName;
            _eventBusPassword = eventBusPassword;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EventBusRabbitMq>()
                .WithParameter(new TypedParameter(typeof(string), _subscriptionClientName))
                .WithParameter(new TypedParameter(typeof(int), _retryCount))
                .As<IEventBus>()
                .SingleInstance();


            var factory = new ConnectionFactory()
            {
                HostName = _eventBusConnection,
                DispatchConsumersAsync = true
            };

            if (!string.IsNullOrEmpty(_eventBusUserName))
                factory.UserName = _eventBusUserName;


            if (!string.IsNullOrEmpty(_eventBusPassword))
                factory.Password = _eventBusPassword;

            builder.Register(c => new DefaultRabbitMqPersistentConnection(
                    factory, c.Resolve<ILogger<DefaultRabbitMqPersistentConnection>>(), _retryCount))
                .As<IRabbitMqPersistentConnection>()
                .SingleInstance();

            builder.RegisterType<InMemoryEventBusSubscriptionsManager>()
                .As<IEventBusSubscriptionsManager>()
                .SingleInstance();
        }
    }
}
