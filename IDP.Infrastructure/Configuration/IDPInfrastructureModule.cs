using IDP.Core.Interfaces;
using IDP.Core.UserAggregate.Entities;
using IDP.Infrastructure.Database;
using IDP.Infrastructure.Initializers;
using IDP.Infrastructure.Repositories;
using IDP.Infrastructure.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace IDP.Infrastructure.Configuration
{
    public static class IDPInfrastructureModule
    {
        public static void AddIDPInfrastructureModule(this IServiceCollection services, IWebHostEnvironment env, string connectionString)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ILocalUserService, LocalUserService>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddDbContext<IdentityDbContext>(options =>
            {

                options.UseSqlServer(connectionString);
                options.UseLazyLoadingProxies();
                if (env.IsDevelopment())
                {
                    ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
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
            //services.AddScoped(x => new IdentityDbContext(connectionString, env.IsDevelopment(), x.GetRequiredService<IMediator>()));
            services.AddHostedService<AdminInitializer>();
            //services.AddScoped(x => new IdentityDbContext(connectionString, env.IsDevelopment(), new Mediator(x.GetRequiredService<ServiceFactory>())));
        }
    }
}
