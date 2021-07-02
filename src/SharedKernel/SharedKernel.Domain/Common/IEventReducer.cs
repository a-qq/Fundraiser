using System.Collections.Generic;

namespace SharedKernel.Domain.Common
{
    public interface IEventReducer
    {
        void ReduceEvents(ICollection<DomainEvent> domainEvents);
    }
}
