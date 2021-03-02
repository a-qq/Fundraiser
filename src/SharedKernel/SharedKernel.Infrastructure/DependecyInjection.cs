using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Infrastructure.Implementations;
using SharedKernel.Infrastructure.Interfaces;
using SharedKernel.Infrastructure.Options;

namespace SharedKernel.Infrastructure
{
    public static class DependecyInjection
    {
        public static void AddSharedKernelInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ISqlConnectionFactory>(x =>
                new SqlConnectionFactory(configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddScoped<IMailManager, MailManager>();
            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<IDateTime, DateTimeService>();
            services.AddSingleton<IAdministratorsProvider, AdministratorsProvider>();
            services.Configure<MailOptions>(configuration.GetSection(MailOptions.MailSettings));
            services.Configure<UrlsOptions>(configuration.GetSection(UrlsOptions.Urls));
            services.Configure<AdministratorsOptions>(configuration.GetSection(AdministratorsOptions.Administrators));
        }
    }
}