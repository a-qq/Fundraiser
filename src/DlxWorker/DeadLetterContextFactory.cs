using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Reflection;

namespace DlxWorker
{
    public class DeadLetterContextFactory : IDesignTimeDbContextFactory<DeadLetterContext>
    {
        public DeadLetterContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<DeadLetterContext>();

            optionsBuilder.UseSqlServer(config.GetConnectionString("DeadLetterExchangeDb"),
                sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(DeadLetterContext).GetTypeInfo().Assembly.GetName().Name);
                });


            return new DeadLetterContext(optionsBuilder.Options);
        }
    }
}