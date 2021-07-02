using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using Dapper;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using SchoolManagement.Application.Common.Interfaces;
using SharedKernel.Infrastructure.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Schools.Queries.GetAuthorizationData
{
    public sealed class
        GetAuthorizationData : IInternalQuery<AuthorizationDto>
    {
        public GetAuthorizationData(Guid schoolId, Guid memberId)
        {
            SchoolId = Guard.Against.Default(schoolId, nameof(schoolId));
            MemberId = Guard.Against.Default(memberId, nameof(memberId));
        }

        public Guid SchoolId { get; }
        public Guid MemberId { get; }
    }

    internal sealed class
        GetFormTutorIdHandler : IRequestHandler<GetAuthorizationData, Maybe<AuthorizationDto>>
    {
        private readonly IMemoryCache _cache;
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public GetFormTutorIdHandler(
            ISqlConnectionFactory sqlConnectionFactory,
            IMemoryCache cache)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
            _cache = cache;
        }

        public async Task<Maybe<AuthorizationDto>> Handle(GetAuthorizationData request,
            CancellationToken cancellationToken)
        {
            if (!_cache.TryGetValue(nameof(GetAuthorizationData) + request.MemberId,
                out AuthorizationDto authorizationData))
            {
                var connection = _sqlConnectionFactory.GetOpenConnection();

                const string sql = "SELECT [M].[Role], " +
                                   "CASE WHEN [M].GroupId = NULL THEN [FT].FormTutorId ELSE NULL END, " +
                                   "CASE WHEN [TR].[TreasurerId] != NULL THEN 1 ELSE 0 END " +
                                   "FROM [management].[Members] AS [M] " +
                                   "LEFT JOIN [management].[Groups] AS [FT] " +
                                   "ON [M].[Id] = [FT].[FormTutorId] " +
                                   "LEFT JOIN [management].[Groups] AS [TR] " +
                                   "ON [M].[Id] = [TR].[TreasurerId] " +
                                   "WHERE [M].[Id] = @MemberId " +
                                   "AND [M].[SchoolId] = @SchoolId";

                authorizationData = await connection.QuerySingleOrDefaultAsync<AuthorizationDto>(sql, new
                {
                    request.MemberId, request.SchoolId
                });

                if (!(authorizationData is null))
                    _cache.Set(SchemaNames.Management + request.MemberId, authorizationData,
                        new MemoryCacheEntryOptions()
                            .SetAbsoluteExpiration(new TimeSpan(0, 0, 2))
                            .SetSlidingExpiration(new TimeSpan(0, 0, 1)));
            }
            
            return authorizationData;
        }
    }
}