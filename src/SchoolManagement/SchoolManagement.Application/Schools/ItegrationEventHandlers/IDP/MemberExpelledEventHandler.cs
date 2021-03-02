using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MediatR;
using SchoolManagement.Domain.SchoolAggregate.Schools.Events;
using SharedKernel.Infrastructure.Implementations;
using SharedKernel.Infrastructure.Interfaces;

namespace SchoolManagement.Application.Schools.ItegrationEventHandlers.IDP
{
    internal sealed class
        MemberExpelledEventHandler : INotificationHandler<DomainEventNotification<MemberExpelledEvent>>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public MemberExpelledEventHandler(
            ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task Handle(DomainEventNotification<MemberExpelledEvent> notification,
            CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;

            const string sqlDelete = "DELETE FROM [auth].[Users] " +
                                     "WHERE [Subject] = @UserId";

            using (var connection = _sqlConnectionFactory.GetOpenConnection())
            {
                await connection.ExecuteAsync(sqlDelete, new
                {
                    UserId = domainEvent.MemberId.ToString()
                });
            }
        }
    }
}