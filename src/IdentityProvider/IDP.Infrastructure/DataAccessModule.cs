using Autofac;
using IDP.Application.Common.Interfaces;
using IDP.Infrastructure.Persistence;
using IDP.Infrastructure.Persistence.Repositories;
using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Infrastructure.Abstractions.Common;
using SharedKernel.Infrastructure.Concretes.Services;
using SK = SharedKernel.Infrastructure.Concretes.IntegrationEventLogEF;

namespace IDP.Infrastructure
{
    public sealed class DataAccessModule : Autofac.Module
    {
        private readonly string _connectionString;

        public DataAccessModule(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UserRepository>()
                .As<IUserRepository>()
                .InstancePerLifetimeScope();

            builder.Register(c =>
                {
                    var options = new DbContextOptionsBuilder<IdentityDbContext>();
                    options.UseSqlServer(_connectionString, sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly(
                                typeof(IdentityDbContext).GetTypeInfo().Assembly.GetName().Name);

                            sqlOptions.EnableRetryOnFailure(
                                maxRetryCount: 15,
                                maxRetryDelay: TimeSpan.FromSeconds(30),
                                errorNumbersToAdd: null);
                        });

                    options.UseLazyLoadingProxies();
                    return new IdentityDbContext(options.Options, c.Resolve<DomainEventService>());
                }).AsSelf()
                .As<IIdentityContext>()
                .As<IUnitOfWork>()
                .InstancePerLifetimeScope();

            builder.Register(c =>
                {
                    var options = new DbContextOptionsBuilder<SK.IntegrationEventLogContext>();
                    options.UseSqlServer(_connectionString,
                        sqlServerOptionsAction: sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly(
                                typeof(IntegrationEventLogContext).GetTypeInfo().Assembly.GetName().Name);

                            sqlOptions.EnableRetryOnFailure(
                                maxRetryCount: 15,
                                maxRetryDelay: TimeSpan.FromSeconds(30),
                                errorNumbersToAdd: null);
                        });

                    return new IntegrationEventLogContext(options.Options);
                }).AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterType<SqlConnectionFactory>()
                .As<ISqlConnectionFactory>()
                .WithParameter(new TypedParameter(typeof(string), _connectionString))
                .InstancePerDependency();
        }
    }
}