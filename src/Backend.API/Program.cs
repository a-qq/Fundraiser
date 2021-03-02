using IDP.Application.Common.Options;
using IDP.Domain.UserAggregate.Entities;
using IDP.Infrastructure.Persistance;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SchoolManagement.Infrastructure.Persistance;
using SharedKernel.Infrastructure.Interfaces;
using SharedKernel.Infrastructure.Options;
using System;
using System.Threading.Tasks;

namespace Backend.API
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var schoolContext = services.GetRequiredService<SchoolContext>();

                    if (schoolContext.Database.IsSqlServer())
                    {
                        schoolContext.Database.Migrate();
                    }

                    var identityContext = services.GetRequiredService<IdentityDbContext>();

                    if (identityContext.Database.IsSqlServer())
                    {
                        schoolContext.Database.Migrate();
                    }

                    await IdentityDbContextSeed.SeedAdministratorsAsync(
                        services.GetRequiredService<IOptions<AdministratorsOptions>>(),
                        services.GetRequiredService<IPasswordHasher<User>>(),
                        services.GetRequiredService<ISqlConnectionFactory>());

                    SharedOptionsValidator.ValidateMailOptions(services.GetRequiredService<IOptions<MailOptions>>());
                    SharedOptionsValidator.ValidateUrlsOptions(services.GetRequiredService<IOptions<UrlsOptions>>());
                    IdpOptionsValidator.ValidateSecurityCodeOptions(services.GetRequiredService<IOptions<SecurityCodeOptions>>());
                }
                catch (Exception ex)
                {
                   // var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

                    //logger.LogError(ex, "An error occurred while migrating or seeding the database.");

                    throw;
                }
            }

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
