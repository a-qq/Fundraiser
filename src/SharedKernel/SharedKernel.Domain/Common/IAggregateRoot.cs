using System.Collections.Generic;

namespace SharedKernel.Domain.Common
{
    public interface IAggregateRoot
    {
        public IReadOnlyList<DomainEvent> DomainEvents { get; }
        public void ClearEvents();
    }
}
