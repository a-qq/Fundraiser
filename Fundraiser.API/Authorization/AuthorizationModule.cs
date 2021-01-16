using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Fundraiser.API.Authorization
{
    public static class AuthorizationModule
    {
        public static void AddAuthorizationHandlers(this IServiceCollection services)
        {
            var assembly = typeof(AuthorizationModule)
                .GetTypeInfo()
                .Assembly;

            services.Scan(scan => scan
                .FromAssemblies(assembly)
                .AddClasses(classes => classes.AssignableTo<IAuthorizationHandler>())
                .AsImplementedInterfaces()
                .WithScopedLifetime());
        }
    }
}
