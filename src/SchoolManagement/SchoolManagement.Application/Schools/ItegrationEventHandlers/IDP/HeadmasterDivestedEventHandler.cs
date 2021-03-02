using Dapper;
using MediatR;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SchoolManagement.Domain.SchoolAggregate.Schools.Events;
using SharedKernel.Infrastructure.Implementations;
using SharedKernel.Infrastructure.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Schools.ItegrationEventHandlers.IDP
{
    internal sealed class HeadmasterDivestedEventHandler : INotificationHandler<DomainEventNotification<HeadmasterDivestedEvent>>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;
        public HeadmasterDivestedEventHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task Handle(DomainEventNotification<HeadmasterDivestedEvent> notification, CancellationToken cancellationToken)
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
                    NewValue = Role.Teacher.ToString(),
                    OldValue = Role.Headmaster.ToString()
                });
            }
        }
    }
}