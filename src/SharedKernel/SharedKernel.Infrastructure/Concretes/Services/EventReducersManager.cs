using Ardalis.GuardClauses;
using Autofac;
using Microsoft.Extensions.Logging;
using SharedKernel.Domain.Common;
using SharedKernel.Infrastructure.Abstractions.Common;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace SharedKernel.Infrastructure.Concretes.Services
{
    public class EventReducersManager : IEventReducersManager
    {
        private const string AUTOFAC_SCOPE_NAME = nameof(EventReducersManager);

        private readonly ILogger<EventReducersManager> _logger;
        private readonly ILifetimeScope _autofac;
        private readonly IReadOnlyDictionary<string, List<Type>> _eventReducers;

        public EventReducersManager(
            Assembly domainAssembly,
            ILifetimeScope autofac,
            ILogger<EventReducersManager> logger)
        {
            _logger = Guard.Against.Null(logger, nameof(logger));
            _autofac = Guard.Against.Null(autofac, nameof(autofac));
            Guard.Against.Null(domainAssembly, nameof(domainAssembly));
            var aggregates = domainAssembly.DefinedTypes.Where(ti =>
                typeof(IAggregateRoot).IsAssignableFrom(ti) && !ti.IsInterface && !ti.IsAbstract);

            _eventReducers = aggregates.ToImmutableDictionary(
                x => Guard.Against.NullOrEmpty(x.Namespace, nameof(x.Namespace)),
                x => x.Assembly.ExportedTypes
                    .Where(ti => typeof(IEventReducer).IsAssignableFrom(ti)
                                 && ti.IsInNamespace(x.Namespace)
                                 && !ti.IsInterface && !ti.IsAbstract)
                    .ToList());
        }

        public IReadOnlyCollection<DomainEvent> ReduceEventsOf(IAggregateRoot aggregate)
        {
            var fullName = Guard.Against.Null(aggregate, nameof(aggregate)).GetType().BaseType?.Namespace;
            var eventReducers = _eventReducers.GetValueOrDefault(fullName);
            if (eventReducers is null)
                return aggregate.DomainEvents.ToList();
            
            using (var scope = _autofac.BeginLifetimeScope(AUTOFAC_SCOPE_NAME))
            {
                var domainEvents = aggregate.DomainEvents.ToList();
                foreach (var eventReducer in eventReducers)
                {
                    var handler = scope.ResolveOptional(eventReducer) as IEventReducer;
                    if (handler is null)
                    {
                        _logger.LogError("EventReducer '{Name}' could not be resolved!", eventReducer.Name);
                        continue;
                    }

                    handler.ReduceEvents(domainEvents);
                }
                
                return domainEvents;
            }
        }
    }
}
