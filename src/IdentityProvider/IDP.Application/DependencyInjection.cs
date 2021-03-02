using IDP.Application.Common.Interfaces;
using IDP.Application.Common.Options;
using IDP.Application.Users;
using IDP.Domain.UserAggregate.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace IDP.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddIdpApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ILocalUserService, LocalUserService>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.Configure<SecurityCodeOptions>(configuration.GetSection(SecurityCodeOptions.SecurityCode));

            return services;
        }
    }
}
