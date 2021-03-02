using SharedKernel.Domain.Common;
using System.Threading.Tasks;

namespace SharedKernel.Infrastructure.Interfaces
{
    public interface IDomainEventService
    {
        Task Publish(DomainEvent domainEvent);
    }
}
