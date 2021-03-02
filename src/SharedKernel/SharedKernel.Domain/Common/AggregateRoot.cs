using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace SharedKernel.Domain.Common
{
    public abstract class AggregateRoot : Entity
    {
        private readonly List<DomainEvent> _domainEvents = new List<DomainEvent>();
        public virtual IReadOnlyList<DomainEvent> DomainEvents => _domainEvents;

        protected virtual void AddDomainEvent(DomainEvent newEvent)
        {
            _domainEvents.Add(newEvent);
        }

        public virtual void ClearEvents()
        {
            _domainEvents.Clear();
        }
    }

    public abstract class AggregateRoot<TId> : Entity<TId>
    {
        private readonly List<DomainEvent> _domainEvents = new List<DomainEvent>();

        protected AggregateRoot(TId id)
            : base(id)
        {
        }

        protected AggregateRoot()
        {
        }

        public virtual IReadOnlyList<DomainEvent> DomainEvents => _domainEvents;

        protected virtual void AddDomainEvent(DomainEvent newEvent)
        {
            _domainEvents.Add(newEvent);
        }

        public virtual void ClearEvents()
        {
            _domainEvents.Clear();
        }
    }
}