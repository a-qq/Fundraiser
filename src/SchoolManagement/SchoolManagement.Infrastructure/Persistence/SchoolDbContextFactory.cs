using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using SharedKernel.Infrastructure.Concretes.TypedIds;
using System.IO;
using System.Reflection;

namespace SchoolManagement.Infrastructure.Persistence
{
    public class SchoolDbContextFactory : IDesignTimeDbContextFactory<SchoolContext>
    {
        public SchoolContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<SchoolContext>();

            optionsBuilder.UseSqlServer(config.GetConnectionString("SchoolManagementDb"),
                sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(SchoolContext).GetTypeInfo().Assembly.GetName().Name);
                });

            optionsBuilder.ReplaceService<IValueConverterSelector, StronglyTypedIdValueConverterSelector>();

            return new SchoolContext(optionsBuilder.Options);
        }
    }
}
