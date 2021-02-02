using Dapper;
using Fundraiser.SharedKernel.Utils;
using MediatR;
using SchoolManagement.Core.SchoolAggregate.Groups;
using SchoolManagement.Core.SchoolAggregate.Schools.Events;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Data.ItegrationHandlers.IDP
{
    internal sealed class TreasurerPromotedEventHandler : INotificationHandler<TreasurerPromotedEvent>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public TreasurerPromotedEventHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task Handle(TreasurerPromotedEvent notification, CancellationToken cancellationToken)
        {
            using (var connection = this._sqlConnectionFactory.GetOpenConnection())
            {
                const string sqlInsert = "INSERT INTO[auth].[Claims]([UserSubject], [Type], [Value]) VALUES " +
                                         "(@UserId, @Type, @Value)";

                await connection.ExecuteAsync(sqlInsert, new
                {
                    UserId = notification.TreasurerId.ToString(),
                    Type = "role",
                    Value = GroupRoles.Treasurer
                }); ;
            }
        }
    }
}
