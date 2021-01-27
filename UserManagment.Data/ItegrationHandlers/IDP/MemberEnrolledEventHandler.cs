using Dapper;
using Fundraiser.SharedKernel.Managers;
using Fundraiser.SharedKernel.Settings;
using Fundraiser.SharedKernel.Utils;
using MediatR;
using Microsoft.Extensions.Options;
using SchoolManagement.Core.SchoolAggregate.Schools.Events;
using SchoolManagement.Data.ItegrationHandlers.IDP;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Data.IntegrationHandlers.IDP
{
    public class MemberEnrolledEventHandler : INotificationHandler<MemberEnrolledEvent>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;
        private readonly IMailManager _mailManager;
        private readonly FrontendSettings _frontendSettings;

        public MemberEnrolledEventHandler(
            ISqlConnectionFactory sqlConnectionFactory,
            IMailManager mailManager,
            IOptions<FrontendSettings> mailOptions)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
            _mailManager = mailManager;
            _frontendSettings = mailOptions.Value;
        }

        public async Task Handle(MemberEnrolledEvent notification, CancellationToken cancellationToken)
        {
            MemberAuthInsertDTO member = null;
            using (var connection = this._sqlConnectionFactory.GetOpenConnection())
            {
                const string sqlQuery = "SELECT [m].Id, [m].FirstName, [m].LastName, " +
                             "[m].Email, [m].SchoolId, [m].Role, [m].Gender " +
                             "FROM [management].[Members] AS [m]" +
                             "WHERE [m].Id = @MemberId";

                member = await connection.QueryFirstOrDefaultAsync<MemberAuthInsertDTO>(sqlQuery, new
                {
                    MemberId = notification.MemberId
                });

                connection.Close();

                if (member == null)
                    return;

                member.GenereteSecurityCode();

                const string sqlInsert = "INSERT INTO [auth].[Users]([Subject], [Email], [SecurityCode], [SecurityCodeIssuedAt], [IsActive]) " +
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
                            Email = member.Email,
                            Code = member.SecurityCode,
                            Issued = DateTime.UtcNow,
                            IsActive = false
                        }, trans);

                        await connection.ExecuteAsync("[auth].[spClaim_InsertSet]", new
                        {
                            claims = claims
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
                var subject = $"{member.FirstName}, welcome in your school's managment system!";
                var url = $"{_frontendSettings.IDPUrl}Registration/Register/?SecurityCode={member.SecurityCode.Replace("+", "%2B")}";
                string body = _mailManager.PopulateWelcomeTemplate(subject, member.FirstName, member.Email, url);
                await _mailManager.SendMailAsync(member.Email, subject, body);
            }
        }
    }
}