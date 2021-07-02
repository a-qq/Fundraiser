using CSharpFunctionalExtensions;
using Dapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharedKernel.Infrastructure.Abstractions.Common;
using SharedKernel.Infrastructure.Abstractions.EventBus;
using SharedKernel.Infrastructure.Concretes.EventBus.RabbitMQ;
using System.Threading.Tasks;

namespace DlxWorker
{
    internal sealed class DeadLetterIntegrationEventHandler : IIntegrationEventHandler<DeadLetterIntegrationEvent>
    {
        private readonly IDateTime _dateTime;
        private readonly ISqlConnectionFactory _sqlConnectionFactory;
        private readonly ILogger<DeadLetterIntegrationEventHandler> _logger;

        public DeadLetterIntegrationEventHandler(
            IDateTime dateTime,
            ISqlConnectionFactory sqlConnectionFactory,
            ILogger<DeadLetterIntegrationEventHandler> logger)
        {
            _dateTime = dateTime;
            _sqlConnectionFactory = sqlConnectionFactory;
            _logger = logger;
        }

        public async Task<Result> Handle(DeadLetterIntegrationEvent @event)
        {
            if (Validate(@event))
            {
                const string sqlQuery = "SELECT TOP 1 1" +
                                   "FROM [dlx].[DeadLetterEvents] AS [Event] " +
                                   "WHERE [Event].[Id] = @Id";

                const string sqlInsert = "INSERT INTO [dlx].[DeadLetterEvents] ([Id], [Type], [Reason], [Content], [CreationTime], [HandledAt]) " +
                                         "VALUES (@Id, @Type, @Reason, @Content, @CreationTime, @HandledAt)";

                using (var connection = _sqlConnectionFactory.GetOpenConnection())
                {
                    var isHandled = await connection.QuerySingleOrDefaultAsync<int?>(sqlQuery, new {Id = @event.Id});
                    if (!isHandled.HasValue)
                    {
                        await connection.ExecuteAsync(sqlInsert, new
                        {
                            Id = @event.Id,
                            Type = @event.Type,
                            Reason = @event.Reason,
                            Content = @event.Content,
                            CreationTime = @event.CreationDate,
                            HandledAt = _dateTime.Now
                        });

                        _logger.LogInformation("Dead letter event (Type: {Type}, Id: {Id}) has been saved with reason: {Reason}", @event.Type, @event.Id, @event.Reason);
                    }
                    else _logger.LogInformation("Dead letter event (Type: {Type}, Id: {Id}) already in the system", @event.Type, @event.Id);

                }
            }
            //always returning true in order not to let dead lettering infinite loop to happen
            return Result.Success();
        }

        private bool Validate(DeadLetterIntegrationEvent @event)
        {
            try
            {
                JObject.Parse(@event.Content);
                return true;
            }
            catch (JsonReaderException ex)
            {
                _logger.LogError(ex, "Event (Type: {Type}) could not be deserialized to JSON. Content: {Content}", @event.Type, @event.Content);
                return false;
            }
        }
    }
}
