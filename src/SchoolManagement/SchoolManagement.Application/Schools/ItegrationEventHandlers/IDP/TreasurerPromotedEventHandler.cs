using Dapper;
using MediatR;
using SchoolManagement.Domain.SchoolAggregate.Groups;
using SchoolManagement.Domain.SchoolAggregate.Schools.Events;
using SharedKernel.Infrastructure.Implementations;
using SharedKernel.Infrastructure.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Schools.ItegrationEventHandlers.IDP
{
    internal sealed class TreasurerPromotedEventHandler : INotificationHandler<DomainEventNotification<TreasurerPromotedEvent>>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public TreasurerPromotedEventHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task Handle(DomainEventNotification<TreasurerPromotedEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;
            using (var connection = _sqlConnectionFactory.GetOpenConnection())
            {
                const string sqlInsert = "INSERT INTO[auth].[Claims]([UserSubject], [Type], [Value]) VALUES " +
                                         "(@UserId, 'role', @Value)";

                await connection.ExecuteAsync(sqlInsert, new
                {
                    UserId = domainEvent.TreasurerId.ToString(),
                    Value = GroupRoles.Treasurer
                }); ;
            }
        }
    }
}
