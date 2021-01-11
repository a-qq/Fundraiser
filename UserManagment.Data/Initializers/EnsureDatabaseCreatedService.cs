using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SchoolManagement.Data.Database;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Data.Initializers
{
    internal sealed class EnsureDatabaseCreatedService : BackgroundService, IHostedService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public EnsureDatabaseCreatedService(
            IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }


        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                SchoolContext schoolContext = scope.ServiceProvider.GetRequiredService<SchoolContext>();
                await schoolContext.Database.MigrateAsync(cancellationToken);
            }
            return;
        }
    }
}
