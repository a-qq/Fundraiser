using System.Threading.Tasks;
using IDP.Application.Common.Options;
using IDP.Domain.UserAggregate.Entities;
using IDP.Infrastructure.Persistance;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SchoolManagement.Infrastructure.Persistance;
using SharedKernel.Infrastructure.Interfaces;
using SharedKernel.Infrastructure.Options;

namespace Backend.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var schoolContext = services.GetRequiredService<SchoolContext>();

                if (schoolContext.Database.IsSqlServer()) schoolContext.Database.Migrate();

                var identityContext = services.GetRequiredService<IdentityDbContext>();

                if (identityContext.Database.IsSqlServer()) schoolContext.Database.Migrate();

                await IdentityDbContextSeed.SeedAdministratorsAsync(
                    services.GetRequiredService<IOptions<AdministratorsOptions>>(),
                    services.GetRequiredService<IPasswordHasher<User>>(),
                    services.GetRequiredService<ISqlConnectionFactory>());

                SharedOptionsValidator.ValidateMailOptions(services.GetRequiredService<IOptions<MailOptions>>());
                SharedOptionsValidator.ValidateUrlsOptions(services.GetRequiredService<IOptions<UrlsOptions>>());
                IdpOptionsValidator.ValidateSecurityCodeOptions(services
                    .GetRequiredService<IOptions<SecurityCodeOptions>>());
            }

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }
    }
}