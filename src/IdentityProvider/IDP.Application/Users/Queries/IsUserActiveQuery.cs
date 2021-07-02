using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using Dapper;
using IDP.Application.Common.Abstractions;
using MediatR;
using SharedKernel.Infrastructure.Abstractions.Common;
using System.Threading;
using System.Threading.Tasks;

namespace IDP.Application.Users.Queries
{
    public sealed class IsUserActiveQuery : IInternalQuery<bool>
    {
        public string Subject { get; }

        public IsUserActiveQuery(string subject)
        {
            Subject = subject;
        }
    }

    internal sealed class IsUserActiveHandler : IRequestHandler<IsUserActiveQuery, Maybe<bool>>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public IsUserActiveHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = Guard.Against.Null(sqlConnectionFactory, nameof(sqlConnectionFactory));
        }

        public async Task<Maybe<bool>> Handle(IsUserActiveQuery request, CancellationToken cancellationToken)
        {
            using var connection = this._sqlConnectionFactory.GetOpenConnection();

            const string sql = "SELECT [User].[IsActive] " +
                               "FROM [auth].Users AS [User] " +
                               "WHERE [User].Subject = @Subject";

            var isActive = await connection.QuerySingleOrDefaultAsync<bool?>(sql, new {request.Subject});

            return isActive ?? Maybe<bool>.None;
        }
    }
}