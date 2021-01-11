using Dapper;
using Fundraiser.SharedKernel.Utils;
using SchoolManagement.Core.Interfaces;
using System;

namespace SchoolManagement.Data.DomainServices
{
    public class EmailUniquenessChecker : IEmailUniquenessChecker
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public EmailUniquenessChecker(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public bool IsUnique(Email email)
        {
            if (email == null)
                throw new ArgumentNullException(nameof(email));

            var connection = this._sqlConnectionFactory.GetOpenConnection();

            const string sql = "SELECT TOP 1 1" +
                               "FROM [management].[Users] AS [User] " +
                               "WHERE [User].[Email] = @Email";

            var usersNumber = connection.QuerySingleOrDefault<int?>(sql,
                            new
                            {
                                Email = email.Value
                            });

            return !usersNumber.HasValue;
        }
    }
}
