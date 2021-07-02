using SharedKernel.Domain.Common;
using System.Collections.Generic;

namespace SharedKernel.Infrastructure.Abstractions.Common
{
    public interface IEventReducersManager
    {
        public IReadOnlyCollection<DomainEvent> ReduceEventsOf(IAggregateRoot aggregate);
    }
}
