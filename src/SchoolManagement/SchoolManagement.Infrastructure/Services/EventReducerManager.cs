using Autofac;
using Microsoft.Extensions.Logging;
using System.Reflection;
using SK = SharedKernel.Infrastructure.Concretes.Services;

namespace SchoolManagement.Infrastructure.Services
{
    public sealed class EventReducersManager : SK.EventReducersManager
    {
        public EventReducersManager(
            Assembly domainAssemblies,
            ILifetimeScope autofac,
            ILogger<SK.EventReducersManager> logger)
            : base(domainAssemblies, autofac, logger)
        {
        }
    }
}
