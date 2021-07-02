using FundraiserManagement.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;
using SharedKernel.Infrastructure.Abstractions.EventBus;
using SharedKernel.Infrastructure.Abstractions.IntegrationEventLogEF;
using System;
using System.Data.Common;
using System.Reflection;
using FundraiserManagement.Application.Common.Interfaces.Services;
using FMA = FundraiserManagement.Application.Common.Interfaces;
using SK = SharedKernel.Infrastructure.Concretes.Services;

namespace FundraiserManagement.Infrastructure.Services
{
    public sealed class IntegrationEventService : SK.IntegrationEventService, IIntegrationEventService
    {
        public IntegrationEventService(FundraiserContext context, IEventBus eventBus,
            Func<DbConnection, Assembly, IIntegrationEventLogService> integrationEventLogServiceFactory,
            ILogger<SK.IntegrationEventService> logger, Assembly integrationEventAssembly, string appName) 
            : base(context, integrationEventAssembly, appName, eventBus, integrationEventLogServiceFactory, logger)
        {
        }
    }
}
