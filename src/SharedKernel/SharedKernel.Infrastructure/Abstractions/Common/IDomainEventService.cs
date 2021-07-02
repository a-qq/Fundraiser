using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SharedKernel.Infrastructure.Abstractions.Common
{
    public interface IDomainEventService
    {
        Task DispatchDomainEvents<TDbContext>(TDbContext domainEvent)
            where TDbContext : DbContext;
    }
}