using Fundraiser.SharedKernel.Managers;
using Fundraiser.SharedKernel.Utils;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Fundraiser.SharedKernel.Configuration
{
    public static class SharedKernelModule
    {
        public static void AddSharedKernelModule(this IServiceCollection services, string connectionString)
        {
            services.AddScoped<ISqlConnectionFactory>(x => new SqlConnectionFactory(connectionString));
            services.AddScoped<IMailManager, MailManager>();
        }
    }
}
