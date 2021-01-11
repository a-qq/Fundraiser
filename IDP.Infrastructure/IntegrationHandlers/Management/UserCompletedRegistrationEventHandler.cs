using Dapper;
using Fundraiser.SharedKernel.Utils;
using IDP.Core.UserAggregate.Events;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace IDP.Infrastructure.IntegrationHandlers.Management
{
    public class UserCompletedRegistrationEventHandler : INotificationHandler<UserCompletedRegistrationEvent>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public UserCompletedRegistrationEventHandler(
            ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task Handle(UserCompletedRegistrationEvent notification, CancellationToken cancellationToken)
        {
            var connection = this._sqlConnectionFactory.GetOpenConnection();

            const string sqlUpdate1 = "UPDATE [management].[Users] SET IsActive = 1 WHERE Id = (@Id)";

            await connection.ExecuteAsync(sqlUpdate1, new
            {
               Id = notification.UserId,
            });
        }

    }
}
