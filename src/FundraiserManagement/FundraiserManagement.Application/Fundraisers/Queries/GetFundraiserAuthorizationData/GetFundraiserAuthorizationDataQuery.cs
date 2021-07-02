using CSharpFunctionalExtensions;
using Dapper;
using FundraiserManagement.Application.Common.Dtos;
using FundraiserManagement.Application.Common.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using FundraiserManagement.Application.Common.Interfaces.Mediator;
using FundraiserManagement.Application.Common.Interfaces.Services;

namespace FundraiserManagement.Application.Fundraisers.Queries.GetFundraiserAuthorizationData
{
    internal sealed class GetFundraiserAuthorizationDataQuery : IInternalQuery<FundraiserAuthDto>
    {
        public Guid SchoolId { get; }
        public Guid FundraiserId { get; }

        public GetFundraiserAuthorizationDataQuery(Guid schoolId, Guid fundraiserId)
        {
            SchoolId = schoolId;
            FundraiserId = fundraiserId;
        }
    }

    internal sealed class GetFundraiserAuthorizationDataQueryHandler : IRequestHandler<GetFundraiserAuthorizationDataQuery, Maybe<FundraiserAuthDto>>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public GetFundraiserAuthorizationDataQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }


        public async Task<Maybe<FundraiserAuthDto>> Handle(GetFundraiserAuthorizationDataQuery request, CancellationToken cancellationToken)
        {
            using var connection = _sqlConnectionFactory.GetOpenConnection();

            const string sql = "SELECT [F].[Type], [F].[GroupId], [F].[ManagerId], [F].[Range] " +
                               "FROM [fund].[Fundraisers] AS [F] " +
                               "WHERE [F].[Id] = @FundraiserId " +
                               "AND [F].[SchoolId] = @SchoolId";

            var authorizationData = await connection.QuerySingleOrDefaultAsync<FundraiserAuthDto>(sql, new
            {
                request.FundraiserId,
                request.SchoolId
            });

            return authorizationData;
        }
    }
}