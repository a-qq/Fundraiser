using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Infrastructure.Persistance;
using SchoolManagement.Infrastructure.Persistance.Repositories;
using SchoolManagement.Infrastructure.Services;
using SharedKernel.Infrastructure.Implementations;

namespace SchoolManagement.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddSchoolManagementInfrastructure(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            services.AddDbContext<SchoolContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(SchoolContext).Assembly.FullName));
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
                options.ReplaceService<IValueConverterSelector, StronglyTypedIdValueConverterSelector>();
            });

            services.AddScoped<ISchoolContext>(provider => provider.GetService<SchoolContext>());
            services.AddScoped<IManagementMailManager, ManagementMailManager>();
            services.AddScoped<ISchoolRepository, SchoolRepository>();
            services.AddScoped<IEmailUniquenessChecker, EmailUniquenessChecker>();
            services.AddScoped<ILogoStorageService, LogoStorageService>();

            return services;
        }
    }
}
