using Dapper;
using Fundraiser.SharedKernel.Utils;
using MediatR;
using SchoolManagement.Core.SchoolAggregate.Schools.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Data.ItegrationHandlers.IDP
{
    internal sealed class SchoolRemovedEventHandler : INotificationHandler<SchoolRemovedEvent>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public SchoolRemovedEventHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task Handle(SchoolRemovedEvent notification, CancellationToken cancellationToken)
        {
            using (var connection = this._sqlConnectionFactory.GetOpenConnection())
            {
                const string sqlDelete = "DELETE u FROM [auth].[Users] u " +
                                         "INNER JOIN [auth].[Claims] c " +
                                         "ON u.[Subject] = c.[UserSubject] " +
                                         "WHERE c.[Type] = 'school_Id' AND " +
                                         "c.[Value] = @Value;";

                await connection.ExecuteAsync(sqlDelete, new
                {
                    Value = notification.SchoolId.ToString()
                });
            }
        }
    }
}
