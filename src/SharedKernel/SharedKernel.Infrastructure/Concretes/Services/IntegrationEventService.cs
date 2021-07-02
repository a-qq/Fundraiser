using Ardalis.GuardClauses;
using Microsoft.Extensions.Logging;
using SharedKernel.Infrastructure.Abstractions.Common;
using SharedKernel.Infrastructure.Abstractions.EventBus;
using SharedKernel.Infrastructure.Abstractions.IntegrationEventLogEF;
using SharedKernel.Infrastructure.Concretes.Models;
using System;
using System.Data.Common;
using System.Reflection;
using System.Threading.Tasks;

namespace SharedKernel.Infrastructure.Concretes.Services
{
    public class IntegrationEventService : IIntegrationEventService
    {
        private readonly IEventBus _eventBus;
        private readonly IUnitOfWork _context;
        private readonly IIntegrationEventLogService _eventLogService;
        private readonly ILogger<IntegrationEventService> _logger;
        private readonly string _appName;

        public IntegrationEventService(
            IUnitOfWork context,
            Assembly integrationEventAssembly,
            string appName,
            IEventBus eventBus,
            Func<DbConnection, Assembly, IIntegrationEventLogService> integrationEventLogServiceFactory,
            ILogger<IntegrationEventService> logger)
        {
            _context = Guard.Against.Null(context, nameof(context));
            _eventBus = Guard.Against.Null(eventBus, nameof(eventBus));
            _eventLogService = Guard.Against.Null(integrationEventLogServiceFactory(
                _context.GetDbConnection(), integrationEventAssembly), "integrationEventLogService");
            _logger = Guard.Against.Null(logger, nameof(logger));
            _appName = Guard.Against.NullOrWhiteSpace(appName, nameof(appName));
        }

        public async Task PublishEventsThroughEventBusAsync(Guid transactionId)
        {
            var pendingLogEvents = await _eventLogService.RetrieveEventLogsPendingToPublishAsync(transactionId);

            foreach (var logEvt in pendingLogEvents)
            {
                _logger.LogInformation("----- Publishing integration event: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", logEvt.EventId, _appName, logEvt.IntegrationEvent);

                try
                {
                    await _eventLogService.MarkEventAsInProgressAsync(logEvt.EventId);
                    _eventBus.Publish(logEvt.IntegrationEvent);
                    await _eventLogService.MarkEventAsPublishedAsync(logEvt.EventId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ERROR publishing integration event: {IntegrationEventId} from {AppName}", logEvt.EventId, _appName);

                    await _eventLogService.MarkEventAsFailedAsync(logEvt.EventId);
                }
            }
        }

        public async Task AddAndSaveEventAsync(IntegrationEvent evt)
        {
            _logger.LogInformation("----- Enqueuing integration event {IntegrationEventId} to repository ({@IntegrationEvent})", evt.Id, evt);

            await _eventLogService.SaveEventAsync(evt, _context.GetCurrentTransaction());
        }
    }
}