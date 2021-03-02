using System.Linq;
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
        GraduationCompletedEventHandler : INotificationHandler<DomainEventNotification<GraduationCompletedEvent>>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public GraduationCompletedEventHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task Handle(DomainEventNotification<GraduationCompletedEvent> notification,
            CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;
            using (var connection = _sqlConnectionFactory.GetOpenConnection())
            {
                const string sqlDeleteRole = "DELETE FROM [auth].[Claims] " +
                                             "WHERE [UserSubject] IN @MemberIds AND " +
                                             "(([Type] = 'role' AND [Value] = @Value) " +
                                             "OR [Type] = 'group_id'";

                if (domainEvent.DivestedFormTutorIds.Any())
                    await connection.ExecuteAsync(sqlDeleteRole, new
                    {
                        MemberIds = domainEvent.DivestedFormTutorIds.Select(x => x.ToString()),
                        Value = GroupRoles.FormTutor
                    });

                if (domainEvent.DivestedTreasurerIds.Any())
                    await connection.ExecuteAsync(sqlDeleteRole, new
                    {
                        MemberIds = domainEvent.DivestedFormTutorIds.Select(x => x.ToString()),
                        Value = GroupRoles.Treasurer
                    });

                if (domainEvent.ArchivedMemberIds.Any())
                {
                    const string sqlDelete = "UPDATE [auth].[Users] " +
                                             "SET IsActive = 0 " +
                                             "WHERE [Subject] IN @Ids";

                    await connection.ExecuteAsync(sqlDelete, new
                    {
                        Ids = domainEvent.ArchivedMemberIds.Select(x => x.ToString())
                    });
                }
            }
        }
    }
}