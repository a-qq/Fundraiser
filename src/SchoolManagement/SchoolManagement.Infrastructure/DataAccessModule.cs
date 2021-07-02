using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Infrastructure.Persistence;
using SchoolManagement.Infrastructure.Persistence.Repositories;
using SchoolManagement.Infrastructure.Services;
using SharedKernel.Infrastructure.Concretes.TypedIds;
using System;
using System.Reflection;
using SK = SharedKernel.Infrastructure.Concretes.IntegrationEventLogEF;

namespace SchoolManagement.Infrastructure
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
            builder.RegisterType<SchoolRepository>()
                .As<ISchoolRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<LocalCacheSchoolRepository>()
                .As<ILocalCacheSchoolRepository>()
                .InstancePerLifetimeScope();

            builder.Register(c =>
                {
                    var options = new DbContextOptionsBuilder<SchoolContext>();
                    options.UseSqlServer(_connectionString,
                        sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly(typeof(SchoolContext).GetTypeInfo().Assembly.GetName().Name);
                            sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30),
                                errorNumbersToAdd: null);
                        });

                    options.UseLazyLoadingProxies();
                    options.ReplaceService<IValueConverterSelector, StronglyTypedIdValueConverterSelector>();
                    
                    return new SchoolContext(options.Options, c.Resolve<DomainEventService>());
                })
                .AsSelf()
                .As<ISchoolContext>()
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
                })
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterType<SqlConnectionFactory>()
                .As<ISqlConnectionFactory>()
                .WithParameter(new TypedParameter(typeof(string), _connectionString))
                .InstancePerDependency();
        }
    }
}