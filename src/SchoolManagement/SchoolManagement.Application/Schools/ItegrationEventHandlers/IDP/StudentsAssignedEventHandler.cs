using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MediatR;
using SchoolManagement.Domain.SchoolAggregate.Schools.Events;
using SharedKernel.Domain.Utils;
using SharedKernel.Infrastructure.Implementations;
using SharedKernel.Infrastructure.Interfaces;

namespace SchoolManagement.Application.Schools.ItegrationEventHandlers.IDP
{
    internal sealed class
        StudentsAssignedEventHandler : INotificationHandler<DomainEventNotification<StudentsAssignedEvent>>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public StudentsAssignedEventHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task Handle(DomainEventNotification<StudentsAssignedEvent> notification,
            CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;
            var claims = DapperBulkOperationsHelper.CreateClaimsInsertTable();
            foreach (var studentId in domainEvent.StudentIds)
                claims.Rows.Add(studentId.ToString(), CustomClaimTypes.GroupId, domainEvent.GroupId.ToString());


            using (var connection = _sqlConnectionFactory.GetOpenConnection())
            {
                await connection.ExecuteAsync("[auth].[spClaim_InsertSet]", new
                {
                    claims
                }, null, null, CommandType.StoredProcedure);
            }
        }
    }
}