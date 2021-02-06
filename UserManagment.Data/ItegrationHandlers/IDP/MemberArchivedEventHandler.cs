using Dapper;
using Fundraiser.SharedKernel.Utils;
using MediatR;
using SchoolManagement.Core.SchoolAggregate.Schools.Events;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Data.ItegrationHandlers.IDP
{
    internal sealed class MemberArchivedEventHandler : INotificationHandler<MemberArchivedEvent>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;
        public MemberArchivedEventHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task Handle(MemberArchivedEvent notification, CancellationToken cancellationToken)
        {
            using (var connection = this._sqlConnectionFactory.GetOpenConnection())
            {
                const string sqlDelete = "UPDATE [auth].[Users] " +
                    "SET IsActive = 0 " +
                    "WHERE [Subject] = @MemberId";

                await connection.ExecuteAsync(sqlDelete, new
                {
                    MemberId = notification.MemberId.ToString()
                });
            }
        }
    }
}
