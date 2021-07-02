using Ardalis.GuardClauses;
using Dapper;
using IdentityModel;
using IDP.Domain.UserAggregate.Entities;
using IDP.Domain.UserAggregate.ValueObjects;
using Microsoft.AspNetCore.Identity;
using SharedKernel.Domain.ValueObjects;
using SharedKernel.Infrastructure.Abstractions.Common;
using SharedKernel.Infrastructure.Options;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace IDP.Infrastructure.Persistence
{
    public static class IdentityDbContextSeed
    {
        public static async Task SeedAdministratorsAsync(AdministratorsOptions adminOptions,
            IPasswordHasher<User> passwordHasher, ISqlConnectionFactory sqlConnectionFactory)
        {
            Guard.Against.Null(adminOptions, nameof(adminOptions));
            Guard.Against.Null(passwordHasher, nameof(passwordHasher));
            Guard.Against.Null(sqlConnectionFactory, nameof(sqlConnectionFactory));

            if (adminOptions.Admins.Count < 1)
                throw new ApplicationException("Administrator(s) data not provided!");

            using (var connection = sqlConnectionFactory.GetOpenConnection())
            {
                foreach (var admin in adminOptions.Admins)
                {
                    var emailValidation = Email.Validate(admin.Email);
                    if (emailValidation.IsFailure)
                        throw new ApplicationException(string.Join(" \n", emailValidation.Error.Errors));

                    if (!Guid.TryParse(admin.Id, out _))
                        throw new ApplicationException($"Id: '{admin.Id}' should be in a guid format!");

                    var passwordValidation = HashedPassword.Validate(admin.Password);
                    if (passwordValidation.IsFailure)
                        throw new ApplicationException(passwordValidation.Error);

                    const string sqlQuery = "SELECT [User].[HashedPassword]" +
                                            "FROM [auth].[Users] AS [User] " +
                                            "WHERE [User].[Email] = @Email OR [User].[Subject] = @Subject";

                    var queriedPasswords = (await connection.QueryAsync<string>(sqlQuery, new
                    {
                        admin.Email,
                        Subject = admin.Id
                    })).AsList();

                    if (queriedPasswords != null && queriedPasswords.Count() > 1)
                        throw new ApplicationException(
                            $"Inconsistency in provided data: Email '{admin.Email}' does not belong to entity with Id: '{admin.Id}'!");

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