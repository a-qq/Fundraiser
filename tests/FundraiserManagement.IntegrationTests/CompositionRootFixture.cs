using FundraiserManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using Serilog.Core;
using SharedKernel.Infrastructure.Concretes.TypedIds;
using System;
using System.IO;
using System.Linq;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using SK = SharedKernel.Infrastructure.Concretes.IntegrationEventLogEF;

namespace FundraiserManagement.IntegrationTests
{
    public class CompositionRootFixture : IDisposable
    {
        private static string ConnectionStringEnvironmentVariableName { get; } =
            "FundraiserManagement_Tests_ConnectionString";

        private readonly IContainer _container;
        public string ConnectionString { get; }
        internal ILifetimeScope BeginLifetimeScope()
        {
            return _container.BeginLifetimeScope();
        }

        public CompositionRootFixture()
        {

            LoadEnvironmentVariables();

            ConnectionString = Environment.GetEnvironmentVariable(ConnectionStringEnvironmentVariableName);
            if (ConnectionString is null)
            {
                throw new ApplicationException(
                    "Define connection string to fundraiser management integration tests database " +
                    $"using environment variable: {ConnectionStringEnvironmentVariableName}");
            }

            MigrateIntegrationLogContext();

            MigrateFundraiserContext();

            Log.Logger = Logger.None;

            _container = Startup.Initialize(
                new ServiceCollection(),
                LoadAppSettings(),
                ConnectionString,
                Log.Logger);
        }

        private static IConfigurationRoot LoadAppSettings()
        {
            return new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", false, true)
                .AddEnvironmentVariables()
                .Build();
        }

        private void MigrateFundraiserContext()
        {
            var fundraiserContextOptionsBuilder = new DbContextOptionsBuilder<FundraiserContext>();

            fundraiserContextOptionsBuilder.UseSqlServer(ConnectionString, options => options.MigrationsAssembly(typeof(FundraiserContext).Assembly.GetName().Name));

            fundraiserContextOptionsBuilder.ReplaceService<IValueConverterSelector, StronglyTypedIdValueConverterSelector>();

            using (var fundraiserContext = new FundraiserContext(fundraiserContextOptionsBuilder.Options))
            {
                fundraiserContext.Database.Migrate();
            }
        }

        private void MigrateIntegrationLogContext()
        {
            var logContextOptionsBuilder = new DbContextOptionsBuilder<SK.IntegrationEventLogContext>();

            logContextOptionsBuilder.UseSqlServer(ConnectionString, options => options.MigrationsAssembly(typeof(IntegrationEventLogContext).Assembly.GetName().Name));

            using (var logContext = new IntegrationEventLogContext(logContextOptionsBuilder.Options))
            {
                logContext.Database.Migrate();
            }
        }

        private static void LoadEnvironmentVariables()
        {
            using (var file = File.OpenText("launchSettings.json"))
            {
                var reader = new JsonTextReader(file);
                var jObject = JObject.Load(reader);

                var variables = jObject.SelectToken("profiles.['IIS Express'].environmentVariables")
                    .Children<JProperty>()
                    .ToList();

                foreach (var variable in variables)
                {
                    Environment.SetEnvironmentVariable(variable.Name, variable.Value.ToString());
                }
            }
        }

        public void Dispose()
        {
            _container.Dispose();
        }
    }
}