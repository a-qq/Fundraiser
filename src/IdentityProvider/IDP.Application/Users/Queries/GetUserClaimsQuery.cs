using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using Dapper;
using IDP.Application.Common.Abstractions;
using MediatR;
using SharedKernel.Infrastructure.Abstractions.Common;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IDP.Application.Users.Queries
{
    public sealed class GetUserClaimsQuery : IInternalQuery<List<ClaimDto>>
    {
        public string Subject { get; }

        public GetUserClaimsQuery(string subject)
        {
            Subject = subject;
        }
    }

    public class ClaimDto
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }

    internal sealed class GetUserClaimsHandler : IRequestHandler<GetUserClaimsQuery, Maybe<List<ClaimDto>>>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public GetUserClaimsHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = Guard.Against.Null(sqlConnectionFactory, nameof(sqlConnectionFactory));   
        }


        public async Task<Maybe<List<ClaimDto>>> Handle(GetUserClaimsQuery request, CancellationToken cancellationToken)
        {
            using var connection = this._sqlConnectionFactory.GetOpenConnection();

            const string sql = "SELECT [Claim].[Type], [Claim].[Value] " +
                               "FROM [auth].[Claims] AS [Claim] " +
                               "WHERE [Claim].[UserSubject] = @Subject";

            var claims = await connection.QueryAsync<ClaimDto>(sql, new { request.Subject });

            return claims.AsList();
        }
    }
}