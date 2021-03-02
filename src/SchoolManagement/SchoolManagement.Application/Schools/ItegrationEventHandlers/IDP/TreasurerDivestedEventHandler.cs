using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MediatR;
using SchoolManagement.Domain.SchoolAggregate.Groups;
using SchoolManagement.Domain.SchoolAggregate.Schools.Events;
using SharedKernel.Infrastructure.Implementations;
using SharedKernel.Infrastructure.Interfaces;

namespace SchoolManagement.Application.Schools.ItegrationEventHandlers.IDP
{
    internal sealed class
        TreasurerDivestedEventHandler : INotificationHandler<DomainEventNotification<TreasurerDivestedEvent>>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public TreasurerDivestedEventHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task Handle(DomainEventNotification<TreasurerDivestedEvent> notification,
            CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;
            using (var connection = _sqlConnectionFactory.GetOpenConnection())
            {
                const string sqlDelete = "DELETE FROM [auth].[Claims] " +
                                         "WHERE [UserSubject] = @MemberId AND " +
                                         "[Type] = 'role' AND [Value] = @Value";

                await connection.ExecuteAsync(sqlDelete, new
                {
                    MemberId = domainEvent.TreasurerId.ToString(),
                    Value = GroupRoles.Treasurer
                });
            }
        }
    }
}