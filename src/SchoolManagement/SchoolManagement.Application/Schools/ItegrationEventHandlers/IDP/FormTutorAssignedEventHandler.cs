using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using IdentityModel;
using MediatR;
using SchoolManagement.Domain.SchoolAggregate.Groups;
using SchoolManagement.Domain.SchoolAggregate.Schools.Events;
using SharedKernel.Domain.Utils;
using SharedKernel.Infrastructure.Implementations;
using SharedKernel.Infrastructure.Interfaces;

namespace SchoolManagement.Application.Schools.ItegrationEventHandlers.IDP
{
    internal sealed class
        FormTutorAssignedEventHandler : INotificationHandler<DomainEventNotification<FormTutorAssignedEvent>>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public FormTutorAssignedEventHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task Handle(DomainEventNotification<FormTutorAssignedEvent> notification,
            CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;
            var claims = DapperBulkOperationsHelper.CreateClaimsInsertTable();
            var subject = domainEvent.MemberId.ToString();
            claims.Rows.Add(subject, JwtClaimTypes.Role, GroupRoles.FormTutor);
            claims.Rows.Add(subject, CustomClaimTypes.GroupId, domainEvent.GroupId.ToString());

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