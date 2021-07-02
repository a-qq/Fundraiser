using Autofac.Extensions.DependencyInjection;
using Destructurama;
using IDP.Application.Common.Options;
using IDP.Domain.UserAggregate.Entities;
using IDP.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using SharedKernel.Infrastructure.Abstractions.Common;
using SharedKernel.Infrastructure.Extensions;
using SharedKernel.Infrastructure.Options;
using System;
using System.IO;
using System.Net;

namespace IDP.Client
{

    public class Program
    {
        public static readonly string Namespace = typeof(Program).Namespace;
        public static readonly string AppName = Namespace.Substring(0, Namespace.IndexOf('.', Namespace.IndexOf('.') + 1) + 1);
        public static int Main(string[] args)
        {
            var configuration = GetConfiguration();
            Log.Logger = CreateSerilogLogger(configuration);

            try
            {
                Log.Information("Configuring web host ({ApplicationContext})...", AppName);
                var host = BuildWebHost(configuration, args);

                SharedOptionsValidator.ValidateMailOptions(host.Services.GetRequiredService<MailOptions>());
                SharedOptionsValidator.ValidateUrlsOptions(host.Services.GetRequiredService<UrlsOptions>());
                IdpOptionsValidator.ValidateSecurityCodeOptions(host.Services
                    .GetRequiredService<SecurityCodeOptions>());

                Log.Information("Applying migrations ({ApplicationContext})...", AppName);
                host.MigrateDbContext<IdentityDbContext>((context, services) =>
                    {
                        var settings = services.GetRequiredService<AdministratorsOptions>();
                        var hasher = services.GetRequiredService<IPasswordHasher<User>>();
                        var factory = services.GetRequiredService<ISqlConnectionFactory>();

                        IdentityDbContextSeed.SeedAdministratorsAsync(settings, hasher, factory).Wait();
                    })
                    .MigrateDbContext<IntegrationEventLogContext>((_, __) => { });

                Log.Information("Starting web host ({ApplicationContext})...", AppName);
                host.Run();

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationContext})!", AppName);
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static ILogger CreateSerilogLogger(IConfiguration configuration)
        {
            var seqServerUrl = configuration["Serilog:SeqServerUrl"];
            var file = configuration["Serilog:File"];

            return new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.WithProperty("ApplicationContext", "IdentityProvider")
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(string.IsNullOrWhiteSpace(file) ? "logs\\idp.log.txt" : file, rollingInterval: RollingInterval.Day)
                .WriteTo.Seq(string.IsNullOrWhiteSpace(seqServerUrl) ? "http://seq" : seqServerUrl)
                .Destructure.UsingAttributes()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }

        private static IHost BuildWebHost(IConfiguration configuration, string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webHostBuilder =>
                {
                    webHostBuilder
                        .CaptureStartupErrors(false)
                        .ConfigureKestrel(options =>
                        {
                            var port = GetDefinedPort(configuration);
                            options.Listen(IPAddress.Any, port,
                                listenOptions => { listenOptions.Protocols = HttpProtocols.Http1AndHttp2; });
                        })
                        .ConfigureAppConfiguration(x => x.AddConfiguration(configuration))
                        .UseStartup<Startup>()
                        .UseContentRoot(Directory.GetCurrentDirectory())
                        .UseSerilog();
                }).Build();

        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            return builder.Build();
        }

        private static int GetDefinedPort(IConfiguration config)
        {
            var port = config.GetValue("PORT", 80);
            return port;
        }
    }
}