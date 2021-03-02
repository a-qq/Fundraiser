using Dapper;
using MediatR;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Domain.SchoolAggregate.Schools.Events;
using SharedKernel.Infrastructure.Implementations;
using SharedKernel.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Schools.ItegrationEventHandlers.IDP
{
    internal sealed class MembersEnrolledEventHandler : INotificationHandler<DomainEventNotification<MembersEnrolledEvent>>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;
        private readonly IManagementMailManager _mailManager;

        public MembersEnrolledEventHandler(
            ISqlConnectionFactory sqlConnectionFactory,
            IManagementMailManager mailManager)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
            _mailManager = mailManager;
        }

        public async Task Handle(DomainEventNotification<MembersEnrolledEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;

            IEnumerable<MemberAuthInsertModel> membersDTO = null;
            using (var connection = _sqlConnectionFactory.GetOpenConnection())
            {
                const string sqlQuery = "SELECT [m].Id, [m].FirstName, [m].LastName, " +
                             "[m].Email, [m].SchoolId, [m].Role, [m].Gender, [m].GroupId " +
                             "FROM [management].[Members] AS [m]" +
                             "WHERE [m].Id IN @memberIds";

                membersDTO = await connection.QueryAsync<MemberAuthInsertModel>(sqlQuery, new
                {
                    memberIds = domainEvent.MemberIds
                });

                connection.Close();

                if (membersDTO == null || !membersDTO.Any())
                    return;

                membersDTO.GenereteSecurityCodes();

                var members = DapperBulkOperationsHelper.GetUsersInsertTable(membersDTO, DateTime.UtcNow);
                var claims = DapperBulkOperationsHelper.GetClaimsInsertTable(membersDTO);

                connection.Open();
                using (var trans = connection.BeginTransaction())
                {
                    try
                    {
                        await connection.ExecuteAsync("[auth].[spUser_InsertSet]", new
                        {
                            users = members
                        }, trans, null, CommandType.StoredProcedure);

                        await connection.ExecuteAsync("[auth].[spClaim_InsertSet]", new
                        {
                            claims
                        }, trans, null, CommandType.StoredProcedure);

                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        const string sqlUpdate = "UPDATE [management].[Users] " +
                                            "SET [GroupId] = NULL " +
                                            "WHERE [Id] IN @MembersId";

                        const string sqlDelete = "DELETE FROM [management].[Users] " +
                                            "WHERE [Id] IN @MembersId";

                        await connection.ExecuteAsync(sqlUpdate, new
                        {
                            MembersId = domainEvent.MemberIds
                        });

                        await connection.ExecuteAsync(sqlDelete, new
                        {
                            MembersId = domainEvent.MemberIds
                        });

                        throw ex;
                    }
                }
            }

            //TODO: test if this fire-forget emails
            var tasks = membersDTO.Select(member => _mailManager.SendRegistrationEmailAsync(member.FirstName, member.Email, member.SecurityCode));
            await Task.WhenAll(tasks);
        }
    }
}

