using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.Moq;
using FundraiserManagement.Application;
using FundraiserManagement.Infrastructure;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Serilog;
using SharedKernel.Infrastructure;
using SharedKernel.Infrastructure.Abstractions.Common;
using SharedKernel.Infrastructure.Abstractions.EventBus;
using SharedKernel.Infrastructure.Abstractions.IntegrationEventLogEF;
using SharedKernel.Infrastructure.Concretes.IntegrationEventLogEF;
using SharedKernel.Infrastructure.Concretes.Services;
using SharedKernel.Infrastructure.Options;
using System.Collections.Generic;
using System.Security.Claims;

namespace FundraiserManagement.IntegrationTests
{
    public static class Startup
    {
        public static IContainer Initialize(
            IServiceCollection services,
            IConfiguration configuration,
            string connectionString,
            ILogger logger)
        {
            //services.AddSingleton(cacheStore);
            services.AddMemoryCache();
            services.AddLogging(loggingBuilder =>
                loggingBuilder.AddSerilog(dispose: true));

            services.AddFundraiserManagementAuthorizationModule();
            services.AddSharedKernelAuthorizationModule();

            var serviceProvider = CreateAutofacServiceProvider(
                services,
                configuration,
                connectionString,
                logger);

            return serviceProvider;
        }

        private static IContainer CreateAutofacServiceProvider(
            IServiceCollection services,
            IConfiguration configuration,
            string connectionString,
            ILogger logger)
        {
            var container = new ContainerBuilder();

            container.Populate(services);
            container.RegisterMock(new Mock<IEventBus>())
                .As<IEventBus>()
                .AsSelf()
                .SingleInstance();

            container.RegisterInstance(logger);

            container.RegisterType<DateTimeService>()
                .As<IDateTime>()
                .InstancePerDependency();

            container.RegisterType<MailManager>()
                .As<IMailManager>()
                .InstancePerDependency();

            container.RegisterType<CurrentUserService>()
                .As<ICurrentUserService>()
                .InstancePerLifetimeScope();

            container.RegisterMock(GetHttpContextAccessorStub())
                .As<IHttpContextAccessor>()
                .AsSelf();

            container.RegisterType<IntegrationEventLogService>()
                .As<IIntegrationEventLogService>()
                .InstancePerDependency();

            container.RegisterType<IdentityService>()
                .As<IIdentityService>()
                .InstancePerLifetimeScope();

            container.RegisterType<DateTimeService>()
                .As<IDateTime>()
                .InstancePerDependency();

            container.RegisterType<AdministratorsProvider>()
                .As<IAdministratorsProvider>()
                .SingleInstance();

            container.RegisterInstance(configuration.GetSection(MailOptions.MailSettings).Get<MailOptions>());
            container.RegisterInstance(configuration.GetSection(UrlsOptions.Urls).Get<UrlsOptions>());
            container.RegisterInstance(configuration.GetSection(AdministratorsOptions.Administrators).Get<AdministratorsOptions>());

            container.RegisterModule(new MediatorModule());
            container.RegisterModule(new ApplicationModule());
            container.RegisterModule(new DataAccessModule(connectionString));
            container.RegisterModule(new ProcessingModule());

            var buildContainer = container.Build();

            //ServiceLocator.SetLocatorProvider(() => new AutofacServiceLocator(buildContainer));

            //var serviceProvider = new AutofacServiceProvider(buildContainer);

            CompositionRoot.SetContainer(buildContainer);

            return buildContainer;
        }

        private static Mock<IHttpContextAccessor> GetHttpContextAccessorStub()
        {
            var stubHttpContextAccessor = new Mock<IHttpContextAccessor>();

            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.NotBefore, "1622747447"),
                new Claim(JwtClaimTypes.Expiration, "9999999999"),
                new Claim(JwtClaimTypes.Issuer, "https://localhost:44309"),
                new Claim(JwtClaimTypes.Audience, "fundraiserapi"),
                new Claim(JwtClaimTypes.Audience, "https://localhost:44309/resources"),
                new Claim(JwtClaimTypes.ClientId, "fundraiserapi_swagger"),
                new Claim(JwtClaimTypes.Subject, "3f0b0a5f-7aa6-46f4-9247-3ef9a9df2407"),
                new Claim(JwtClaimTypes.AuthenticationTime, "1622743448"),
                new Claim(JwtClaimTypes.IdentityProvider, "local"),
                new Claim(JwtClaimTypes.Role, "Administrator"),
                new Claim(JwtClaimTypes.JwtId, "5CB887B330AB296FCCF46CC49AF2649D"),
                new Claim(JwtClaimTypes.SessionId, "7896DCF199D0D75E28E39B9CECCEDF80"),
                new Claim(JwtClaimTypes.IssuedAt, "1622747447"),
                new Claim(JwtClaimTypes.Scope, "fundraiserapi.fullaccess"),
                new Claim(JwtClaimTypes.AuthenticationMethod, "pwd")
            };

            var context = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(
                    new ClaimsIdentity(claims))
            };

            stubHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(context);

            return stubHttpContextAccessor;
        }
    }
}