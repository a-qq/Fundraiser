using Dapper;
using Fundraiser.SharedKernel.Utils;
using MediatR;
using SchoolManagement.Core.SchoolAggregate.Groups;
using SchoolManagement.Core.SchoolAggregate.Schools.Events;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Data.ItegrationHandlers.IDP
{
    public sealed class FormTutorAssignedEventHandler : INotificationHandler<FormTutorAssignedEvent>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public FormTutorAssignedEventHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task Handle(FormTutorAssignedEvent notification, CancellationToken cancellationToken)
        {
            var connection = this._sqlConnectionFactory.GetOpenConnection();

            const string sqlInsert = "INSERT INTO[auth].[Claims]([UserSubject], [Type], [Value]) VALUES " +
                                     "(@UserId, @Type, @Value)";

            await connection.ExecuteAsync(sqlInsert, new
            {
                UserId = notification.MemberId.ToString(),
                Type = "role",
                Value = GroupRoles.FormTutor
            });

        }
    }
}
