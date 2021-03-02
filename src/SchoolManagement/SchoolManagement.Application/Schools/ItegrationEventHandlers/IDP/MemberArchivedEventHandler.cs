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
        MemberArchivedEventHandler : INotificationHandler<DomainEventNotification<MemberArchivedEvent>>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public MemberArchivedEventHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task Handle(DomainEventNotification<MemberArchivedEvent> notification,
            CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;
            using (var connection = _sqlConnectionFactory.GetOpenConnection())
            {
                const string sqlDelete = "UPDATE [auth].[Users] " +
                                         "SET IsActive = 0 " +
                                         "WHERE [Subject] = @MemberId";

                await connection.ExecuteAsync(sqlDelete, new
                {
                    MemberId = domainEvent.MemberId.ToString()
                });
            }
        }
    }
}