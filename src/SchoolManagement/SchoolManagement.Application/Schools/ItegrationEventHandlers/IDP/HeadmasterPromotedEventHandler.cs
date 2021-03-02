using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MediatR;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SchoolManagement.Domain.SchoolAggregate.Schools.Events;
using SharedKernel.Infrastructure.Implementations;
using SharedKernel.Infrastructure.Interfaces;

namespace SchoolManagement.Application.Schools.ItegrationEventHandlers.IDP
{
    internal sealed class
        HeadmasterPromotedEventHandler : INotificationHandler<DomainEventNotification<HeadmasterPromotedEvent>>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public HeadmasterPromotedEventHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task Handle(DomainEventNotification<HeadmasterPromotedEvent> notification,
            CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;
            using (var connection = _sqlConnectionFactory.GetOpenConnection())
            {
                const string sqlUpdate = "UPDATE FROM [auth].[Claims] " +
                                         "SET [Value] = @NewValue " +
                                         "WHERE [UserSubject] = @MemberId AND " +
                                         "[Type] = 'role' AND [Value] = @OldValue";

                await connection.ExecuteAsync(sqlUpdate, new
                {
                    MemberId = domainEvent.HeadmasterId.ToString(),
                    NewValue = Role.Headmaster.ToString(),
                    OldValue = Role.Teacher.ToString()
                });
            }
        }
    }
}