using Autofac;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Infrastructure.Abstractions.EventBus;
using System;
using System.Reflection;

namespace DlxWorker
{
    public sealed class DeadLetterExchangeModule : Autofac.Module
    {
        private readonly string _connectionString;

        public DeadLetterExchangeModule(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(DeadLetterIntegrationEventHandler).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IIntegrationEventHandler<>));

            builder.Register(c =>
                {
                    var options = new DbContextOptionsBuilder<DeadLetterContext>();
                    options.UseSqlServer(_connectionString, sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(
                            typeof(DeadLetterContext).GetTypeInfo().Assembly.GetName().Name);

                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 15,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    });

                    return new DeadLetterContext(options.Options);
                }).AsSelf()
                .InstancePerLifetimeScope();
        }
    }
}