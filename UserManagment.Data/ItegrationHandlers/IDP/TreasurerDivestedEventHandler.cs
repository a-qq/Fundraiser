﻿using Dapper;
using Fundraiser.SharedKernel.Utils;
using MediatR;
using SchoolManagement.Core.SchoolAggregate.Groups;
using SchoolManagement.Core.SchoolAggregate.Schools.Events;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Data.ItegrationHandlers.IDP
{
    internal sealed class TreasurerDivestedEventHandler : INotificationHandler<TreasurerDivestedEvent>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;
        public TreasurerDivestedEventHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task Handle(TreasurerDivestedEvent notification, CancellationToken cancellationToken)
        {
            using (var connection = this._sqlConnectionFactory.GetOpenConnection())
            {
                const string sqlDelete = "DELETE FROM [auth].[Claims] " +
                                         "WHERE [UserSubject] = @MemberId AND " +
                                         "[Type] = @Type AND [Value] = @Value";

                await connection.ExecuteAsync(sqlDelete, new
                {
                    MemberId = notification.TreasurerId.ToString(),
                    Type = "role",
                    Value = GroupRoles.Treasurer
                });
            }
        }
    }
}