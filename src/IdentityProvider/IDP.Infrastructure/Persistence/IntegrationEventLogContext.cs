using Microsoft.EntityFrameworkCore;
using SK = SharedKernel.Infrastructure.Concretes.IntegrationEventLogEF;

namespace IDP.Infrastructure.Persistence
{
    public sealed class IntegrationEventLogContext : SK.IntegrationEventLogContext
    {
        public IntegrationEventLogContext(
            DbContextOptions<SK.IntegrationEventLogContext> options) 
            : base(options)
        {
        }
    }
}