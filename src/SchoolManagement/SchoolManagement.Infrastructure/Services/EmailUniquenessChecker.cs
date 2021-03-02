using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SchoolManagement.Application.Common.Interfaces;
using SharedKernel.Domain.ValueObjects;
using SharedKernel.Infrastructure.Interfaces;

namespace SchoolManagement.Infrastructure.Services
{
    internal sealed class EmailUniquenessChecker : IEmailUniquenessChecker
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public EmailUniquenessChecker(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
            SqlMapper.AddTypeHandler(new EmailTypeHandler());
        }

        public async Task<bool> IsUnique(Email email)
        {
            if (email == null)
                throw new ArgumentNullException(nameof(email));

            var connection = _sqlConnectionFactory.GetOpenConnection();

            const string sql = "SELECT TOP 1 1" +
                               "FROM [management].[Members] AS [Member] " +
                               "WHERE [Member].[Email] = @Email";

            var usersNumber = await connection.QuerySingleOrDefaultAsync<int?>(sql,
                new
                {
                    Email = email.Value
                });

            return !usersNumber.HasValue;
        }

        public async Task<Tuple<bool, IEnumerable<Email>>> AreUnique(IEnumerable<Email> emails)
        {
            if (emails == null || !emails.Any())
                throw new ArgumentNullException(nameof(emails));

            var connection = _sqlConnectionFactory.GetOpenConnection();
            const string sql = "SELECT [Member].[Email] " +
                               "FROM [management].[Members] AS [Member] " +
                               "WHERE [Member].[Email] IN @Emails";

            var emailsAstrings = emails.Select(e => e.Value);

            var duplicates = await connection.QueryAsync<Email>(sql,
                new
                {
                    Emails = emailsAstrings
                });

            return new Tuple<bool, IEnumerable<Email>>(!duplicates.Any(), duplicates);
        }
    }

    public class EmailTypeHandler : SqlMapper.TypeHandler<Email>
    {
        public override Email Parse(object value)
        {
            return Email.Create(value.ToString()).Value;
        }

        public override void SetValue(IDbDataParameter parameter, Email email)
        {
            parameter.Direction = ParameterDirection.Output;
            parameter.DbType = DbType.String;
            parameter.Value = email.Value;
        }
    }
}