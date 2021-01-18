using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SchoolManagement.Core.Interfaces;
using SchoolManagement.Data.Database;
using SchoolManagement.Data.DomainServices;
using SchoolManagement.Data.Initializers;
using SchoolManagement.Data.Repositories;
using SchoolManagement.Data.Services;

namespace SchoolManagement.Data.Configuration
{
    public static class SchoolManagementInfrastructureModule
    {
        public static void AddSchoolManagementInfrastructureModule(this IServiceCollection services, IWebHostEnvironment env, string connectionString)
        {
            services.AddScoped<ISchoolRepository, SchoolRepository>();
            services.AddScoped<IEmailUniquenessChecker, EmailUniquenessChecker>();
            services.AddDbContext<SchoolContext>(options =>
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
            services.AddScoped<IAuthorizationService, AuthorizationService>();
            services.AddScoped<IStorageService, LocalStorageService>();
            services.AddHostedService<EnsureDatabaseCreatedService>();
        }
    }
}
