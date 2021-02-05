using Dapper;
using Fundraiser.SharedKernel.Utils;
using MediatR;
using SchoolManagement.Core.SchoolAggregate.Groups;
using SchoolManagement.Core.SchoolAggregate.Schools.Events;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Data.ItegrationHandlers.IDP
{
    internal sealed class GraduationCompletedEventHandler : INotificationHandler<GraduationCompletedEvent>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;
        public GraduationCompletedEventHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task Handle(GraduationCompletedEvent notification, CancellationToken cancellationToken)
        {
            using (var connection = this._sqlConnectionFactory.GetOpenConnection())
            {
                const string sqlDeleteRole = "DELETE FROM [auth].[Claims] " +
                         "WHERE [UserSubject] IN @MemberIds AND " +
                         "[Type] = 'role' AND [Value] = @Value";

                if (notification.IdsOfDivestedFormTutors.Any())
                {
                    await connection.ExecuteAsync(sqlDeleteRole, new
                    {
                        MemberIds = notification.IdsOfDivestedFormTutors.Select(x => x.ToString()),
                        Value = GroupRoles.FormTutor
                    });
                }

                if (notification.IdsOfDivestedTreasurers.Any())
                {
                    await connection.ExecuteAsync(sqlDeleteRole, new
                    {
                        MemberIds = notification.IdsOfDivestedFormTutors.Select(x => x.ToString()),
                        Value = GroupRoles.Treasurer
                    });
                }

                if (notification.IdsOfArchivedStudents.Any())
                {
                    const string sqlDelete = "UPDATE [auth].[Users] " +
                        "SET IsActive = 0 " +
                        "WHERE [Subject] IN @Ids";

                    await connection.ExecuteAsync(sqlDelete, new
                    {
                        Ids = notification.IdsOfArchivedStudents.Select(x => x.ToString())
                    });
                }
            }
        }
    }
}