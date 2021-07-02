using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel.Domain.Common;
using SharedKernel.Infrastructure.Abstractions.Common;
using SharedKernel.Infrastructure.Concretes.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SharedKernel.Infrastructure.Concretes.Services
{
    public class DomainEventService : IDomainEventService
    {
        private readonly ILogger<DomainEventService> _logger;
        private readonly IPublisher _mediator;
        private readonly IEventReducersManager _eventReducer;

        public DomainEventService(
            ILogger<DomainEventService> logger, 
            IPublisher mediator,
            IEventReducersManager eventReducer)
        {
            _logger = logger;
            _mediator = mediator;
            _eventReducer = eventReducer;
        }

        public async Task DispatchDomainEvents<TDbContext>(TDbContext ctx)
            where TDbContext : DbContext
        {
                var aggregates = ctx.ChangeTracker
                    .Entries<IAggregateRoot>()
                    .Where(ar => ar.Entity.DomainEvents.Any())
                    .Select(ar => ar.Entity)
                    .ToList();

                foreach (var aggregate in aggregates)
                {
                    while (true)
                    {
                        var reducedEvents = _eventReducer.ReduceEventsOf(aggregate);
                        aggregate.ClearEvents();
                        foreach (var domainEvent in reducedEvents)
                        {
                            _logger.LogInformation("Publishing domain event. Event - {event}",
                                domainEvent.GetType().Name);
                            await _mediator.Publish(GetNotificationCorrespondingToDomainEvent(domainEvent));
                        }

                        if (!aggregate.DomainEvents.Any())
                            break;
                    }
                }
        }

        private static INotification GetNotificationCorrespondingToDomainEvent(DomainEvent domainEvent)
        {
            return (INotification) Activator.CreateInstance(
                typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType()), domainEvent);
        }
    }
}