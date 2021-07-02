using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace SharedKernel.Domain.Common
{
    public abstract class AggregateRoot : Entity, IAggregateRoot
    {
        private readonly List<DomainEvent> _domainEvents = new List<DomainEvent>();
        public virtual IReadOnlyList<DomainEvent> DomainEvents => _domainEvents;

        protected void AddDomainEvent(DomainEvent newEvent)
        {
            _domainEvents.Add(newEvent);
        }

        public void ClearEvents()
        {
            _domainEvents.Clear();
        }
    }

    public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot
        where TId : ITypedId
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