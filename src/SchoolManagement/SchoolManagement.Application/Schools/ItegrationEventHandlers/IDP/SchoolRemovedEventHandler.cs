using Dapper;
using MediatR;
using SchoolManagement.Domain.SchoolAggregate.Schools.Events;
using SharedKernel.Infrastructure.Implementations;
using SharedKernel.Infrastructure.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Schools.ItegrationEventHandlers.IDP
{
    internal sealed class SchoolRemovedEventHandler : INotificationHandler<DomainEventNotification<SchoolRemovedEvent>>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public SchoolRemovedEventHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task Handle(DomainEventNotification<SchoolRemovedEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;
            using (var connection = _sqlConnectionFactory.GetOpenConnection())
            {
                const string sqlDelete = "DELETE u FROM [auth].[Users] u " +
                                         "INNER JOIN [auth].[Claims] c " +
                                         "ON u.[Subject] = c.[UserSubject] " +
                                         "WHERE c.[Type] = 'school_id' AND " +
                                         "c.[Value] = @Value;";

                await connection.ExecuteAsync(sqlDelete, new
                {
                    Value = domainEvent.SchoolId.ToString()
                });
            }
        }
    }
}
