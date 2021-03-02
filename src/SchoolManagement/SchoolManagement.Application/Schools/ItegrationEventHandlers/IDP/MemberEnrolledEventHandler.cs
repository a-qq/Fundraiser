using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MediatR;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Domain.SchoolAggregate.Schools.Events;
using SharedKernel.Infrastructure.Implementations;
using SharedKernel.Infrastructure.Interfaces;

namespace SchoolManagement.Application.Schools.ItegrationEventHandlers.IDP
{
    internal sealed class
        MemberEnrolledEventHandler : INotificationHandler<DomainEventNotification<MemberEnrolledEvent>>
    {
        private readonly IManagementMailManager _mailManager;
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public MemberEnrolledEventHandler(
            ISqlConnectionFactory sqlConnectionFactory,
            IManagementMailManager mailManager)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
            _mailManager = mailManager;
        }

        public async Task Handle(DomainEventNotification<MemberEnrolledEvent> notification,
            CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;
            using (var connection = _sqlConnectionFactory.GetOpenConnection())
            {
                const string sqlQuery = "SELECT [m].Id, [m].FirstName, [m].LastName, " +
                                        "[m].Email, [m].SchoolId, [m].Role, [m].Gender " +
                                        "FROM [management].[Members] AS [m]" +
                                        "WHERE [m].Id = @MemberId";

                var member = await connection.QueryFirstOrDefaultAsync<MemberAuthInsertModel>(sqlQuery, new
                {
                    domainEvent.MemberId
                });

                connection.Close();

                if (member == null)
                    return;

                member.GenereteSecurityCode();

                const string sqlInsert =
                    "INSERT INTO [auth].[Users]([Subject], [Email], [SecurityCode], [SecurityCodeIssuedAt], [IsActive]) " +
                    "VALUES (@Subject, @Email, @Code, @Issued, @IsActive)";

                var claims = DapperBulkOperationsHelper.GetClaimsInsertTable(member);

                connection.Open();
                using (var trans = connection.BeginTransaction())
                {
                    try
                    {
                        await connection.ExecuteAsync(sqlInsert, new
                        {
                            Subject = member.Id.ToString(),
                            member.Email,
                            Code = member.SecurityCode,
                            Issued = DateTime.UtcNow,
                            IsActive = false
                        }, trans);

                        await connection.ExecuteAsync("[auth].[spClaim_InsertSet]", new
                        {
                            claims
                        }, trans, null, CommandType.StoredProcedure);

                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        const string sqlDelete = "DELETE FROM [management].[Members] " +
                                                 "WHERE [Id] = @MemberId";
                        await connection.ExecuteAsync(sqlDelete, new
                        {
                            MemberId = member.Id
                        });

                        throw ex;
                    }
                }

                await _mailManager.SendRegistrationEmailAsync(member.FirstName, member.Email, member.SecurityCode);
            }
        }
    }
}