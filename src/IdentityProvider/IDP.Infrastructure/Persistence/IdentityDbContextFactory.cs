using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Reflection;

namespace IDP.Infrastructure.Persistence
{
    public class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
    {
        public IdentityDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();

            optionsBuilder.UseSqlServer(config.GetConnectionString("IdentityProviderDb"),
                sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(IdentityDbContext).GetTypeInfo().Assembly.GetName().Name);
                });

            return new IdentityDbContext(optionsBuilder.Options);
        }
    }
}