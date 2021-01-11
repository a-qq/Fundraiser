using Dapper;
using Fundraiser.SharedKernel.Utils;
using IdentityModel;
using IDP.Core.UserAggregate.Entities;
using IDP.Infrastructure.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IDP.Infrastructure.Initializers
{
    internal sealed class AdminInitializer : BackgroundService, IHostedService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        //private readonly IPasswordHasher<User> _passwordHasher;
        //private readonly IdentityDbContext _identityContext;
        //private readonly ISqlConnectionFactory _sqlConnectionFactory;
        public AdminInitializer(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        //public AdminInitializer(
        //    IdentityDbContext identityContext,
        //    ISqlConnectionFactory sqlConnectionFactory,
        //    IPasswordHasher<User> passwordHasher) 
        //{
        //    _passwordHasher = passwordHasher;
        //    _identityContext = identityContext;
        //    _sqlConnectionFactory = sqlConnectionFactory;
        //}
        //public async Task StartAsync(CancellationToken cancellationToken)
        //{
        //    await _identityContext.Database.MigrateAsync(cancellationToken);
        //    if (!await _identityContext.Users.AnyAsync(u => u.Subject == "3f0b0a5f-7aa6-46f4-9247-3ef9a9df2407"))
        //    {
        //        string password = Environment.GetEnvironmentVariable("AdminPassword");
        //        var hashedPassword = _passwordHasher.HashPassword(null, password);
        //        var connection = _sqlConnectionFactory.GetOpenConnection();
        //        const string sqlInsert = "INSERT INTO [auth].[Users] ([Subject], [Email], [HashedPassword], [IsActive]) VALUES " +
        //                            "(@Subject, @Email, @Password, @IsActive)" +
        //                            "INSERT INTO [auth].Claims([UserSubject], [Type], [Value] VALUES " +
        //                            "(@Subject, @Type, @Value)";
        //        await connection.ExecuteAsync(sqlInsert, new
        //        {
        //            Subject = "3f0b0a5f-7aa6-46f4-9247-3ef9a9df2407",
        //            Email = Environment.GetEnvironmentVariable("AdminEmail"),
        //            Password = hashedPassword,
        //            IsActive = true,
        //            Type = JwtClaimTypes.Role,
        //            Value = "Administrator"
        //        });
        //    }
        //}

        //public Task StopAsync(CancellationToken cancellationToken)
        //{
        //        return Task.CompletedTask;
        //}

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                ISqlConnectionFactory sqlConnectionFactory = scope.ServiceProvider.GetRequiredService<ISqlConnectionFactory>();
                IPasswordHasher<User> passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();
                IdentityDbContext identityContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
                await identityContext.Database.MigrateAsync(cancellationToken);
                if (!await identityContext.Users.AnyAsync(u => u.Subject == "3f0b0a5f-7aa6-46f4-9247-3ef9a9df2407", cancellationToken))
                {
                    string password = Environment.GetEnvironmentVariable("AdminPassword");
                    var hashedPassword = passwordHasher.HashPassword(null, password);
                    var connection = sqlConnectionFactory.GetOpenConnection();
                    const string sqlInsert = "INSERT INTO [auth].[Users] ([Subject], [Email], [HashedPassword], [IsActive]) VALUES " +
                                        "(@Subject, @Email, @HashedPassword, @IsActive)" +
                                        "INSERT INTO [auth].[Claims]([UserSubject], [Type], [Value]) VALUES " +
                                         "(@UserSubject, @Type, @Value)";

                    await connection.ExecuteAsync(sqlInsert, new
                    {
                        Subject = "3f0b0a5f-7aa6-46f4-9247-3ef9a9df2407",
                        Email = Environment.GetEnvironmentVariable("AdminEmail"),
                        HashedPassword = hashedPassword,
                        IsActive = true,
                        UserSubject = "3f0b0a5f-7aa6-46f4-9247-3ef9a9df2407",
                        Type = JwtClaimTypes.Role,
                        Value = "Administrator"
                    });
                }
                return;
            }
        }
    }
}
