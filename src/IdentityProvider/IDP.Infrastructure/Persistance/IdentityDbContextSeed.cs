using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using IdentityModel;
using IDP.Domain.UserAggregate.Entities;
using IDP.Domain.UserAggregate.ValueObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SharedKernel.Domain.ValueObjects;
using SharedKernel.Infrastructure.Interfaces;
using SharedKernel.Infrastructure.Options;

namespace IDP.Infrastructure.Persistance
{
    public static class IdentityDbContextSeed
    {
        public static async Task SeedAdministratorsAsync(IOptions<AdministratorsOptions> adminOptions,
            IPasswordHasher<User> passwordHasher, ISqlConnectionFactory sqlConnectionFactory)
        {
            if (adminOptions is null)
                throw new ArgumentNullException(nameof(adminOptions));

            if (passwordHasher is null)
                throw new ArgumentNullException(nameof(passwordHasher));

            if (sqlConnectionFactory is null)
                throw new ArgumentNullException(nameof(sqlConnectionFactory));

            var adminData = adminOptions.Value;
            if (adminData.Admins.Count < 1)
                throw new ApplicationException("Administrator(s) data not provided!");

            using (var connection = sqlConnectionFactory.GetOpenConnection())
            {
                foreach (var admin in adminData.Admins)
                {
                    var emailValidation = Email.Validate(admin.Email);
                    if (emailValidation.IsFailure)
                        throw new ApplicationException(string.Join(" \n", emailValidation.Error.Errors));

                    if (!Guid.TryParse(admin.Id, out _))
                        throw new ApplicationException($"Id: '{admin.Id}' should be in a guid format!");

                    var passwordValidation = HashedPassword.Validate(admin.Password);
                    if (passwordValidation.IsFailure)
                        throw new ApplicationException(string.Join(" \n", passwordValidation.Error.Errors));

                    const string sqlQuery = "SELECT [User].[HashedPassword]" +
                                            "FROM [auth].[Users] AS [User] " +
                                            "WHERE [User].[Email] = @Email OR [User].[Subject] = @Subject";

                    var queriedPasswords = await connection.QueryAsync<string>(sqlQuery, new
                    {
                        admin.Email,
                        Subject = admin.Id
                    });

                    if (queriedPasswords != null && queriedPasswords.Count() > 1)
                        throw new ApplicationException(
                            $"Inconsistency in provided data: Email '{admin.Email}' does not belong to eniity with Id: '{admin.Id}'!");

                    var queriedPassword = queriedPasswords.SingleOrDefault();

                    if (queriedPassword is null)
                    {
                        connection.Close();

                        var hash = passwordHasher.HashPassword(null, admin.Password);

                        connection.Open();

                        using (var trans = connection.BeginTransaction())
                        {
                            try
                            {
                                const string sqlAdminInsert =
                                    "INSERT INTO [auth].[Users] ([Subject], [Email], [HashedPassword], [IsActive]) " +
                                    "VALUES (@Subject, @Email, @Password, 1)";

                                await connection.ExecuteAsync(sqlAdminInsert, new
                                {
                                    Subject = admin.Id,
                                    admin.Email,
                                    Password = hash
                                }, trans);

                                const string sqlClaimsInsert =
                                    "INSERT INTO [auth].[Claims] ([UserSubject], [Type], [Value]) VALUES " +
                                    "(@Subject, @Type, @Value)";

                                await connection.ExecuteAsync(sqlClaimsInsert, new
                                {
                                    Subject = admin.Id,
                                    Type = JwtClaimTypes.Role,
                                    Value = "Administrator"
                                }, trans);

                                trans.Commit();
                            }
                            catch (Exception ex)
                            {
                                trans.Rollback();
                                throw new ApplicationException($"Could not insert {admin.Email}!", ex);
                            }
                        }
                    }
                    else
                    {
                        connection.Close();
                        var result = passwordHasher.VerifyHashedPassword(null, queriedPassword, admin.Password);
                        if (result == PasswordVerificationResult.Failed)
                        {
                            var hash = passwordHasher.HashPassword(null, admin.Password);
                            connection.Open();
                            const string sqlUpdate = "UPDATE [auth].[Users] " +
                                                     "SET [HashedPassword] = @Password " +
                                                     "WHERE [Subject] = @Subject";

                            await connection.ExecuteAsync(sqlUpdate, new
                            {
                                Subject = admin.Id,
                                Password = hash
                            });
                        }

                        if (connection.State == ConnectionState.Closed)
                            connection.Open();
                    }
                }
            }
        }
    }
}