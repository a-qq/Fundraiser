using Dapper;
using IDP.Domain.UserAggregate.Events;
using MediatR;
using SharedKernel.Infrastructure.Implementations;
using SharedKernel.Infrastructure.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace IDP.Application.EventHandlers.Integration.Management
{
    internal sealed class UserCompletedRegistrationEventHandler : INotificationHandler<DomainEventNotification<UserCompletedRegistrationEvent>>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public UserCompletedRegistrationEventHandler(
            ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task Handle(DomainEventNotification<UserCompletedRegistrationEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;
            using (var connection = _sqlConnectionFactory.GetOpenConnection())
            {
                const string sqlUpdate1 = "UPDATE [management].[Members] SET IsActive = 1 WHERE Id = (@Id)";

                await connection.ExecuteAsync(sqlUpdate1, new
                {
                    Id = domainEvent.UserId,
                });
            }
        }

    }
}
