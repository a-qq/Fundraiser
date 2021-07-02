using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using SK = SharedKernel.Infrastructure.Concretes.IntegrationEventLogEF;

namespace IDP.Infrastructure.Persistence
{
    public class IntegrationEventLogContextFactory : IDesignTimeDbContextFactory<IntegrationEventLogContext>
    {
        public IntegrationEventLogContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<SK.IntegrationEventLogContext>();

            optionsBuilder.UseSqlServer(config.GetConnectionString("IdentityProviderDb"), options => options.MigrationsAssembly(typeof(IntegrationEventLogContextFactory).Assembly.GetName().FullName));

            return new IntegrationEventLogContext(optionsBuilder.Options);
        }
    }
}