using Autofac;
using FundraiserManagement.Application.Common.Interfaces;
using FundraiserManagement.Infrastructure.Persistence;
using FundraiserManagement.Infrastructure.Persistence.Repositories;
using FundraiserManagement.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SharedKernel.Infrastructure.Concretes.TypedIds;
using System;
using System.Reflection;
using FundraiserManagement.Application.Common.Interfaces.Services;
using SK = SharedKernel.Infrastructure.Concretes.IntegrationEventLogEF;

namespace FundraiserManagement.Infrastructure
{
    public sealed class DataAccessModule : Autofac.Module
    {
        private readonly string _connectionString;

        public DataAccessModule(string connectionString)
        {
            this._connectionString = connectionString;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FundraiserRepository>()
                .As<IFundraiserRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<MemberRepository>()
                .As<IMemberRepository>()
                .InstancePerLifetimeScope();

            builder.Register(c =>
                {
                    var dbContextOptionsBuilder = new DbContextOptionsBuilder<FundraiserContext>();
                    dbContextOptionsBuilder.UseSqlServer(_connectionString,
                        sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly(typeof(FundraiserContext).GetTypeInfo().Assembly.GetName().Name);
                            sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30),
                                errorNumbersToAdd: null);
                        });
                    dbContextOptionsBuilder
                        .ReplaceService<IValueConverterSelector, StronglyTypedIdValueConverterSelector>();

                    return new FundraiserContext(dbContextOptionsBuilder.Options, c.Resolve<DomainEventService>());
                }).AsSelf()
                .As<IFundraiserContext>()
                .InstancePerLifetimeScope();

            builder.Register(c =>
                {
                    var dbContextOptionsBuilder = new DbContextOptionsBuilder<SK.IntegrationEventLogContext>();
                    dbContextOptionsBuilder.UseSqlServer(_connectionString,
                        sqlServerOptionsAction: sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly(typeof(IntegrationEventLogContext).GetTypeInfo().Assembly
                                .GetName().Name);
                            sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30),
                                errorNumbersToAdd: null);
                        });

                    return new IntegrationEventLogContext(dbContextOptionsBuilder.Options);
                }).AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterType<SqlConnectionFactory>()
                .As<ISqlConnectionFactory>()
                .WithParameter(new TypedParameter(typeof(string), _connectionString))
                .InstancePerDependency();
        }
    }
}
