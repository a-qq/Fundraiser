using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using SharedKernel.Infrastructure.Concretes.TypedIds;
using System.IO;
using System.Reflection;

namespace FundraiserManagement.Infrastructure.Persistence
{
    public class FundraiserContextFactory : IDesignTimeDbContextFactory<FundraiserContext>
    {
        public FundraiserContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<FundraiserContext>();

            optionsBuilder.UseSqlServer(config.GetConnectionString("FundraiserManagementDb"),
                sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(FundraiserContext).GetTypeInfo().Assembly.GetName().Name);
                });

            optionsBuilder.ReplaceService<IValueConverterSelector, StronglyTypedIdValueConverterSelector>();

            return new FundraiserContext(optionsBuilder.Options);
        }
    }
}