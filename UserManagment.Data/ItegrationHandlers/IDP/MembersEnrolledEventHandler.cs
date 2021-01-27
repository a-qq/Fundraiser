using Dapper;
using Fundraiser.SharedKernel.Managers;
using Fundraiser.SharedKernel.Settings;
using Fundraiser.SharedKernel.Utils;
using MediatR;
using Microsoft.Extensions.Options;
using SchoolManagement.Core.SchoolAggregate.Schools.Events;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Data.ItegrationHandlers.IDP
{
    public sealed class MembersEnrolledEventHandler : INotificationHandler<MembersEnrolledEvent>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;
        private readonly IMailManager _mailManager;
        private readonly FrontendSettings _frontendSettings;

        public MembersEnrolledEventHandler(
            ISqlConnectionFactory sqlConnectionFactory,
            IMailManager mailManager,
            IOptions<FrontendSettings> mailOptions)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
            _mailManager = mailManager;
            _frontendSettings = mailOptions.Value;
        }

        public async Task Handle(MembersEnrolledEvent notification, CancellationToken cancellationToken)
        {
            IEnumerable<MemberAuthInsertDTO> membersDTO = null;
            using (var connection = this._sqlConnectionFactory.GetOpenConnection())
            {
                const string sqlQuery = "SELECT [m].Id, [m].FirstName, [m].LastName, " +
                             "[m].Email, [m].SchoolId, [m].Role, [m].Gender " +
                             "FROM [management].[Members] AS [m]" +
                             "WHERE [m].Id IN @memberIds";

                membersDTO = await connection.QueryAsync<MemberAuthInsertDTO>(sqlQuery, new
                {
                    memberIds = notification.MemberIds
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
                            claims = claims
                        }, trans, null, CommandType.StoredProcedure);

                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        const string sqlDelete = "DELETE FROM [management].[Users] " +
                                     "WHERE [Id] IN @MembesrId";

                        await connection.ExecuteAsync(sqlDelete, new
                        {
                            MembesrId = notification.MemberIds
                        });

                        throw ex;
                    }
                }
            }

            foreach(var member in membersDTO)
            {
                var subject = $"{member.FirstName}, welcome in your school's managment system!";
                var url = $"{_frontendSettings.IDPUrl}Registration/Register/?SecurityCode={member.SecurityCode.Replace("+", "%2B")}";
                string body = _mailManager.PopulateWelcomeTemplate(subject, member.FirstName, member.Email, url);
                await _mailManager.SendMailAsync(member.Email, "{user.FirstName}, welcome in your school's managment system!", body);
            }
        }
    }
}
