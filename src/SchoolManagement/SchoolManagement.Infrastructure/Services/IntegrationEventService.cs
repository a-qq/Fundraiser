using Microsoft.Extensions.Logging;
using SchoolManagement.Infrastructure.Persistence;
using SharedKernel.Infrastructure.Abstractions.EventBus;
using SharedKernel.Infrastructure.Abstractions.IntegrationEventLogEF;
using System;
using System.Data.Common;
using System.Reflection;
using SMA = SchoolManagement.Application.Common.Interfaces;
using SK = SharedKernel.Infrastructure.Concretes.Services;

namespace SchoolManagement.Infrastructure.Services
{
    public sealed class IntegrationEventService : SK.IntegrationEventService, SMA.IIntegrationEventService
    {
        public IntegrationEventService(SchoolContext context, IEventBus eventBus,
            Func<DbConnection, Assembly, IIntegrationEventLogService> integrationEventLogServiceFactory,
            ILogger<SK.IntegrationEventService> logger, Assembly integrationEventAssembly, string appName) 
            : base(context, integrationEventAssembly, appName, eventBus, integrationEventLogServiceFactory, logger)
        {
        }
    }
}