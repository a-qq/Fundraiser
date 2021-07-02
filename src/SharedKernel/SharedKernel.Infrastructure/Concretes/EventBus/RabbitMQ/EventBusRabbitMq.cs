using Autofac;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using SharedKernel.Infrastructure.Abstractions.EventBus;
using SharedKernel.Infrastructure.Abstractions.EventBus.RabbitMQ;
using SharedKernel.Infrastructure.Concretes.Models;
using SharedKernel.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Infrastructure.Concretes.EventBus.RabbitMQ
{
    public class EventBusRabbitMq : IEventBus, IDisposable
    {
        private const string BROKER_NAME = "event_bus";
        private const string AUTOFAC_SCOPE_NAME = "event_bus";
        public const string DEAD_LETTER_QUEUE_NAME = "dlx_queue";
        public const string DEAD_LETTER_EXCHANGE_NAME = "dlx_exchange";

        private readonly IRabbitMqPersistentConnection _persistentConnection;
        private readonly ILogger<EventBusRabbitMq> _logger;
        private readonly IEventBusSubscriptionsManager _subsManager;
        private readonly ILifetimeScope _autofac;
        private readonly int _retryCount;

        private IModel _consumerChannel;
        private string _queueName;
        private bool IsDlx(string eventName) => eventName == nameof(DeadLetterIntegrationEvent);

        public EventBusRabbitMq(IRabbitMqPersistentConnection persistentConnection, ILogger<EventBusRabbitMq> logger,
            ILifetimeScope autofac, IEventBusSubscriptionsManager subsManager, string queueName = null, int retryCount = 5)
        {
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
            _queueName = queueName;
            _consumerChannel = CreateConsumerChannel();
            _autofac = autofac;
            _retryCount = retryCount;
            _subsManager.OnEventRemoved += SubsManager_OnEventRemoved;
        }

        private void SubsManager_OnEventRemoved(object sender, string eventName)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            bool isDlx = IsDlx(eventName);

            using (var channel = _persistentConnection.CreateModel())
            {
                channel.QueueUnbind(queue: isDlx ? DEAD_LETTER_QUEUE_NAME : _queueName,
                    exchange: isDlx ? DEAD_LETTER_EXCHANGE_NAME : BROKER_NAME,
                    routingKey: isDlx ? string.Empty : eventName); ;

                if (_subsManager.IsEmpty)
                {
                    _queueName = string.Empty;
                    _consumerChannel.Close();
                }
            }
        }

        public void Publish(IntegrationEvent @event)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var policy = RetryPolicy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogWarning(ex, "Could not publish event: {EventId} after {Timeout}s ({ExceptionMessage})", @event.Id, $"{time.TotalSeconds:n1}", ex.Message);
                });

            var eventName = @event.GetType().Name;

            _logger.LogTrace("Creating RabbitMQ channel to publish event: {EventId} ({EventName})", @event.Id, eventName);

            using (var channel = _persistentConnection.CreateModel())
            {
                _logger.LogTrace("Declaring RabbitMQ exchange to publish event: {EventId}", @event.Id);

                var args = new Dictionary<string, object>
                {
                    {"x-message-tll", 30000}
                };

                channel.ExchangeDeclare(exchange: BROKER_NAME, type: ExchangeType.Direct, arguments: args);

                var message = JsonConvert.SerializeObject(@event);
                var body = Encoding.UTF8.GetBytes(message);

                policy.Execute(() =>
                {
                    var properties = channel.CreateBasicProperties();
                    properties.DeliveryMode = 2; // persistent

                    _logger.LogTrace("Publishing event to RabbitMQ: {EventId}", @event.Id);

                    channel.BasicPublish(
                        exchange: BROKER_NAME,
                        routingKey: eventName,
                        mandatory: true,
                        basicProperties: properties,
                        body: body);
                });
            }
        }

        public void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = _subsManager.GetEventKey<T>();
            DoInternalSubscription(eventName);

            _logger.LogInformation("Subscribing to event {EventName} with {EventHandler}", eventName, typeof(TH).GetGenericTypeName());

            _subsManager.AddSubscription<T, TH>();
            StartBasicConsume();
        }

        private void DoInternalSubscription(string eventName)
        {
            var containsKey = _subsManager.HasSubscriptionsForEvent(eventName);
            if (!containsKey)
            {
                if (!_persistentConnection.IsConnected)
                {
                    _persistentConnection.TryConnect();
                }

                using (var channel = _persistentConnection.CreateModel())
                {
                    bool isDlx = IsDlx(eventName);
                    channel.QueueBind(queue: isDlx ? DEAD_LETTER_QUEUE_NAME : _queueName,
                                      exchange: isDlx ? DEAD_LETTER_EXCHANGE_NAME : BROKER_NAME,
                                      routingKey: isDlx ? string.Empty : eventName);
                }
            }
        }

        public void Unsubscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = _subsManager.GetEventKey<T>();

            _logger.LogInformation("Unsubscribing from event {EventName}", eventName);

            _subsManager.RemoveSubscription<T, TH>();
        }

        public void Dispose()
        {
            if (!(_consumerChannel is null))
            {
                _consumerChannel.Dispose();
            }

            _subsManager.Clear();
        }

        private void StartBasicConsume()
        {
            _logger.LogTrace("Starting RabbitMQ basic consume");

            if (!(_consumerChannel is null))
            {
                var consumer = new AsyncEventingBasicConsumer(_consumerChannel);

                consumer.Received += Consumer_Received;

                _consumerChannel.BasicConsume(
                    queue: _queueName,
                    autoAck: false,
                    consumer: consumer);

                if(_queueName == "eSchool")
                    _consumerChannel.BasicConsume(
                        queue: DEAD_LETTER_QUEUE_NAME,
                        autoAck: true,
                        consumer: consumer);
            }
            else
            {
                _logger.LogError("StartBasicConsume can't call on _consumerChannel == null");
            }
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
        {
            //
            var eventName = eventArgs.RoutingKey;
            var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
            string dlxReason = null;
            if (eventArgs.BasicProperties.Headers?.ContainsKey("x-first-death-reason") ?? false)
            {
                var reasonBytes = eventArgs.BasicProperties.Headers["x-first-death-reason"] as byte[];
                dlxReason = Encoding.UTF8.GetString(reasonBytes);
            }

            try
            {
                if (message.ToLowerInvariant().Contains("throw-fake-exception"))
                {
                    throw new InvalidOperationException($"Fake exception requested: \"{message}\"");
                }

                var result = await ProcessEvent(eventName, message, dlxReason);
                if (result.IsFailure)
                {
                    _logger.LogError("Dead lettering RabbitMQ message: \"{Message}\". Reason: {Error}.", message, result.Error);
                    _consumerChannel.BasicReject(eventArgs.DeliveryTag, false);
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dead lettering RabbitMQ message: \"{Message}\". Reason: {Reason}", message, ex.Message);
                _consumerChannel.BasicReject(eventArgs.DeliveryTag, false);
                return;
            }

            // Even on exception we take the message off the queue.
            // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX). 
            // For more information see: https://www.rabbitmq.com/dlx.html
            _consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
        }

        private IModel CreateConsumerChannel()
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            _logger.LogTrace("Creating RabbitMQ consumer channel");

            var channel = _persistentConnection.CreateModel();


            channel.ExchangeDeclare(exchange: BROKER_NAME,
                type: ExchangeType.Direct);

            channel.ExchangeDeclare(exchange: DEAD_LETTER_EXCHANGE_NAME,
                type: ExchangeType.Fanout);


            channel.QueueDeclare(queue: DEAD_LETTER_QUEUE_NAME,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var args = new Dictionary<string, object>()
            {
                {"x-dead-letter-exchange", DEAD_LETTER_EXCHANGE_NAME},
            };

            channel.QueueDeclare(queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: args);


            channel.CallbackException += (sender, ea) =>
            {
                _logger.LogWarning(ea.Exception, "Recreating RabbitMQ consumer channel");

                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();
                StartBasicConsume();
            };

            return channel;
        }

        private async Task<Result> ProcessEvent(string eventName, string message, string dlxReason)
        {
            var isDlx = !string.IsNullOrWhiteSpace(dlxReason);
            var processingEventName = isDlx ? nameof(DeadLetterIntegrationEvent) : eventName;

            if (isDlx)
                _logger.LogTrace("Processing RabbitMQ event: {EventName} with original name {OriginalEventName}", processingEventName, eventName);
            else
                _logger.LogTrace("Processing RabbitMQ event: {EventName}", processingEventName);

            if (_subsManager.HasSubscriptionsForEvent(processingEventName))
            {
                await using (var scope = _autofac.BeginLifetimeScope(AUTOFAC_SCOPE_NAME))
                {
                    var subscriptions = _subsManager.GetHandlersForEvent(processingEventName);

                    var results = await Task.WhenAll(
                        subscriptions.Select(s =>
                            HandleTypedSubscription(processingEventName, message, scope, s, dlxReason)));

                    var finalResult = Result.Combine(results, "\n");

                    return finalResult;
                }
            }

            _logger.LogError("No subscription for RabbitMQ event: {EventName}", processingEventName);

            return Result.Failure($"No subscription for RabbitMQ event: {processingEventName}!");
        }

        private async Task<Result> HandleTypedSubscription(string eventName, string message, ILifetimeScope scope,
            InMemoryEventBusSubscriptionsManager.SubscriptionInfo subscription, string dlxReason)
        {
            var handler = scope.ResolveOptional(subscription.HandlerType);
            if (handler == null)
            {
                _logger.LogError("Handler for RabbitMQ event: {EventName} could not be resolved!",
                    eventName);

                return Result.Failure("Handler for RabbitMQ event: {eventName} could not be resolved!");
            }

            var eventType = _subsManager.GetEventTypeByName(eventName);

            var integrationEvent = DeserializeEventObject(eventName, message, eventType, dlxReason);

            var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

            await Task.Yield();

            return await (Task<Result>)concreteType.GetMethod("Handle")
                .Invoke(handler, new object[] { integrationEvent });
        }

        private object DeserializeEventObject(string eventName, string message, Type eventType, string dlxReason)
        {
            if (!IsDlx(eventName))
                return JsonConvert.DeserializeObject(message, eventType);

            var helper = JsonConvert.DeserializeObject(message, eventType) as DeadLetterIntegrationEvent;
            var dlxEvent = new DeadLetterIntegrationEvent(helper.Id, helper.CreationDate, eventName, message, dlxReason);

            return dlxEvent;
        }
    }
}
