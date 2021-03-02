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
        FormTutorDivestedEventHandler : INotificationHandler<DomainEventNotification<FormTutorDivestedEvent>>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public FormTutorDivestedEventHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task Handle(DomainEventNotification<FormTutorDivestedEvent> notification,
            CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;
            using (var connection = _sqlConnectionFactory.GetOpenConnection())
            {
                const string sqlDelete = "DELETE FROM [auth].[Claims] " +
                                         "WHERE [UserSubject] = @MemberId AND " +
                                         "(([Type] = 'role' AND [Value] = @Value) " +
                                         "OR [Type] = 'group_id')";

                await connection.ExecuteAsync(sqlDelete, new
                {
                    MemberId = domainEvent.MemberId.ToString(),
                    Value = GroupRoles.FormTutor
                });
            }
        }
    }
}