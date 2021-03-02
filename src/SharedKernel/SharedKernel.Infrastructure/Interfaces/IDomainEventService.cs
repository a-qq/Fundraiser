using System.Threading.Tasks;
using SharedKernel.Domain.Common;

namespace SharedKernel.Infrastructure.Interfaces
{
    public interface IDomainEventService
    {
        Task Publish(DomainEvent domainEvent);
    }
}