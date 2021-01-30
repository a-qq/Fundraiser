using Dapper;
using Fundraiser.SharedKernel.Utils;
using MediatR;
using SchoolManagement.Core.SchoolAggregate.Schools.Events;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Data.ItegrationHandlers.IDP
{
    internal sealed class MemberExpelledEventHandler : INotificationHandler<MemberExpelledEvent>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public MemberExpelledEventHandler(
            ISqlConnectionFactory sqlConnectionFactory)
        {
            this._sqlConnectionFactory = sqlConnectionFactory;
        }
        public async Task Handle(MemberExpelledEvent notification, CancellationToken cancellationToken)
        {
            const string sqlDelete = "DELETE FROM [auth].[Users] " +
                                     "WHERE [Subject] = @UserId";

            using (var connection = this._sqlConnectionFactory.GetOpenConnection())
            {
                await connection.ExecuteAsync(sqlDelete, new
                {
                    UserId = notification.MemberId.ToString()
                }); 
            }
        }
    }
}
