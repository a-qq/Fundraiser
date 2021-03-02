//using Dapper;
//using Fundraiser.SharedKernel.Utils;
//using IDP.Domain.UserAggregate.Events;
//using MediatR;
//using System.Threading;
//using System.Threading.Tasks;

//namespace  IDP.Application.EventHandlers.Integration.Fundraiser
//{
//    public class UserCompletedRegistrationEventHandler : INotificationHandler<UserCompletedRegistrationEvent>
//    {
//        private readonly ISqlConnectionFactory _sqlConnectionFactory;

//        public UserCompletedRegistrationEventHandler(
//            ISqlConnectionFactory sqlConnectionFactory)
//        {
//            _sqlConnectionFactory = sqlConnectionFactory;
//        }

//        public async Task Handle(UserCompletedRegistrationEvent notification, CancellationToken cancellationToken)
//        {
//            var connection = this._sqlConnectionFactory.GetOpenConnection();

//            //Insert actice user to fundraiser schema
//            const string sqlUpdate1 = "";// = "UPDATE [management].[Users] SET IsActive = 1 WHERE UserId = (@UserId)";

//            await connection.ExecuteAsync(sqlUpdate1, new
//            {
//                notification.UserId,
//            });
//        }

//    }
//}
