using IDP.Application.Common.Interfaces;
using IDP.Infrastructure.Persistance;
using IDP.Infrastructure.Persistance.Repositories;
using IDP.Infrastructure.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IDP.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddIdpInfrastructure(
            this IServiceCollection services, IWebHostEnvironment env, IConfiguration configuration)
        {
            services.AddDbContext<IdentityDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(IdentityDbContext).Assembly.FullName));
                options.UseLazyLoadingProxies();
                if (env.IsDevelopment())
                {
                    var loggerFactory = LoggerFactory.Create(builder =>
                    {
                        builder
                            .AddFilter((category, level) =>
                                category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Information)
                            .AddConsole();
                    });
                    options.UseLoggerFactory(loggerFactory)
                        .EnableSensitiveDataLogging();
                }
            });

            services.AddScoped<IIdentityContext>(provider => provider.GetService<IdentityDbContext>());
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IIdpMailManager, IdpMailManager>();

            return services;
        }
    }
}