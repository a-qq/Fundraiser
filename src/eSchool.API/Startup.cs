using Autofac;
using Autofac.Extensions.DependencyInjection;
using DlxWorker;
using eSchool.API.Extensions;
using eSchool.API.Filters;
using eSchool.API.Middleware;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SchoolManagement.Application.IntegrationEvents.Events;
using SharedKernel.Infrastructure;
using SharedKernel.Infrastructure.Abstractions.Common;
using SharedKernel.Infrastructure.Abstractions.EventBus;
using SharedKernel.Infrastructure.Abstractions.EventBus.RabbitMQ;
using SharedKernel.Infrastructure.Concretes.EventBus;
using SharedKernel.Infrastructure.Concretes.EventBus.RabbitMQ;
using SharedKernel.Infrastructure.Concretes.Services;
using SharedKernel.Infrastructure.Options;
using System.Text.Json.Serialization;
using FundraiserManagement.Application;
using SharedKernel.Infrastructure.Abstractions.IntegrationEventLogEF;
using SharedKernel.Infrastructure.Concretes.IntegrationEventLogEF;

namespace eSchool.API
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }
        public IConfigurationRoot Configuration { get; }
        public ILifetimeScope AutofacContainer { get; private set; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = (IConfigurationRoot)configuration;
            Environment = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews(options => { options.Filters.Add<RouteIdsValidationFilter>(); })
                .ConfigureApiBehaviorOptions(options => { options.SuppressModelStateInvalidFilter = true; })
                .AddFluentValidation(fv => { fv.RunDefaultMvcValidationAfterFluentValidationExecutes = false; })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    //options.JsonSerializerOptions.IgnoreNullValues = true;
                });
            //services.AddOptions();
            //services.AddEventBus(Configuration);
            //services.AddSchoolManagementApplication();
            //services.AddSchoolManagementInfrastructure(Configuration, Environment);
            //services.AddFundraiserInfrastructure(Configuration, Environment);
            //services.AddSharedKernel(Configuration);
            //services.AddDeadLetterModule(Configuration);
            //services.AddSharedKernelAuthorizationModule();
            services.AddHttpContextAccessor();
            services.AddSwaggerConfiguration();
            services.AddSharedKernelAuthorizationModule();
            services.AddFundraiserManagementAuthorizationModule();
            //services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
            //    .AddIdentityServerAuthentication(options =>
            //    {
            //        options.Authority = EnvironmentVariables.IdpUrl;
            //        options.ApiName = "fundraiserapi";
            //        //options.ApiSecret = EnvironmentVariables.JwtSecret;
            //    });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new DeadLetterExchangeModule(
                Configuration.GetConnectionString("DeadLetterExchangeDb")));

            builder.RegisterModule(new SchoolManagement.Application.ApplicationModule());
            builder.RegisterModule(new SchoolManagement.Application.MediatorModule());
            builder.RegisterModule(new SchoolManagement.Infrastructure.ProcessingModule());
            builder.RegisterModule(new SchoolManagement.Infrastructure.DataAccessModule(
                Configuration.GetConnectionString("SchoolManagementDb")));

            builder.RegisterModule(new FundraiserManagement.Application.MediatorModule());
            builder.RegisterModule(new FundraiserManagement.Application.ApplicationModule());
            builder.RegisterModule(new FundraiserManagement.Infrastructure.ProcessingModule());
            builder.RegisterModule(new FundraiserManagement.Infrastructure.DataAccessModule(
                Configuration.GetConnectionString("FundraiserManagementDb")));

            builder.RegisterModule(new RabbitMqEventBusModule(
                Configuration["EventBus:SubscriptionClientName"],
                int.TryParse(Configuration["EventBus:RetryCount"], out int retryCount) ? retryCount : 5,
                Configuration["EventBus:Connection"],
                Configuration["EventBus:UserName"],
                Configuration["EventBus:Password"]));

            builder.RegisterType<IntegrationEventLogService>()
                .As<IIntegrationEventLogService>()
                .InstancePerDependency();

            builder.RegisterType<DateTimeService>()
                .As<IDateTime>()
                .InstancePerDependency();

            builder.RegisterType<MailManager>()
                .As<IMailManager>()
                .InstancePerDependency();

            builder.RegisterType<CurrentUserService>()
                .As<ICurrentUserService>()
                .InstancePerLifetimeScope();

            builder.RegisterType<IdentityService>()
                .As<IIdentityService>()
                .InstancePerLifetimeScope();

            builder.RegisterType<DateTimeService>()
                .As<IDateTime>()
                .InstancePerDependency();

            builder.RegisterType<AdministratorsProvider>()
                .As<IAdministratorsProvider>()
                .SingleInstance();

            builder.RegisterInstance(Configuration.GetSection(MailOptions.MailSettings).Get<MailOptions>());
            builder.RegisterInstance(Configuration.GetSection(UrlsOptions.Urls).Get<UrlsOptions>());
            builder.RegisterInstance(Configuration.GetSection(AdministratorsOptions.Administrators).Get<AdministratorsOptions>());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            this.AutofacContainer = app.ApplicationServices.GetAutofacRoot();

            app.UseMiddleware<ExceptionHandler>();

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Fundraiser v1");
                options.OAuthClientId("fundraiserapi_swagger");
                options.OAuthAppName("Backend.API - Swagger");
                options.OAuthUsePkce();
            });

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            ConfigureEventBus(app);
        }


        private void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            eventBus.SetFundraiserManagementIntegrationEventSubscriptions();
            eventBus.Subscribe<UserRegisteredIntegrationEvent, IIntegrationEventHandler<UserRegisteredIntegrationEvent>>();
            eventBus.Subscribe<DeadLetterIntegrationEvent, IIntegrationEventHandler<DeadLetterIntegrationEvent>>();
        }
    }

    static class CustomExtensionsMethods
    {
        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            var subscriptionClientName = configuration["EventBus:SubscriptionClientName"];

            services.AddSingleton<IEventBus, EventBusRabbitMq>(sp =>
            {
                var rabbitMqPersistentConnection = sp.GetRequiredService<IRabbitMqPersistentConnection>();
                var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                var logger = sp.GetRequiredService<ILogger<EventBusRabbitMq>>();
                var eventBusSubscriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                var retryCount = 5;
                if (!string.IsNullOrEmpty(configuration["EventBus:RetryCount"]))
                {
                    retryCount = int.Parse(configuration["EventBus:RetryCount"]);
                }

                return new EventBusRabbitMq(rabbitMqPersistentConnection, logger, iLifetimeScope, eventBusSubscriptionsManager, subscriptionClientName, retryCount);
            });


            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

            return services;
        }

        public static IServiceCollection AddSharedKernel(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ISqlConnectionFactory>(x =>
                new SqlConnectionFactory(configuration.GetConnectionString("DefaultConnection")));

            //services.AddDbContext<IntegrationEventLogContext>(options =>
            //{
            //    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
            //        sqlServerOptionsAction: sqlOptions =>
            //        {
            //            sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
            //            //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
            //            sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
            //        });
            //});

            //services.AddTransient<IIntegrationEventService, IntegrationEventService>();

            //services.AddTransient<Func<DbContext, IRequestManager>>(
            //    sp => (DbContext c) => new RequestManager(c));

            //services.AddTransient<Func<DbConnection, Assembly, IIntegrationEventLogService>>(
            //    sp => (DbConnection c, Assembly a) => new IntegrationEventLogService(c, a));

            //services.AddSingleton<IEventReducersManager>(sp =>
            //{
            //    var logger = sp.GetRequiredService<ILogger<EventReducersManager>>();

            //    var domainAssemblies = new List<Assembly>
            //        {typeof(School).GetTypeInfo().Assembly};

            //    var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();

            //    return new EventReducersManager(domainAssemblies, iLifetimeScope, logger);
            //});

            //services.AddSingleton<IRabbitMqPersistentConnection>(sp =>
            //{
            //    var logger = sp.GetRequiredService<ILogger<DefaultRabbitMqPersistentConnection>>();

            //    var factory = new ConnectionFactory()
            //    {
            //        HostName = Configuration["EventBus:Connection"],
            //        DispatchConsumersAsync = true
            //    };

            //    if (!string.IsNullOrEmpty(Configuration["EventBus:UserName"]))
            //    {
            //        factory.UserName = Configuration["EventBus:UserName"];
            //    }

            //    if (!string.IsNullOrEmpty(Configuration["EventBus:Password"]))
            //    {
            //        factory.Password = Configuration["EventBus:Password"];
            //    }

            //    var retryCount = 5;
            //    if (!string.IsNullOrEmpty(Configuration["EventBus:RetryCount"]))
            //    {
            //        retryCount = int.Parse(Configuration["EventBus:RetryCount"]);
            //    }

            //    return new DefaultRabbitMqPersistentConnection(factory, logger, retryCount);
            //});

            //services.AddScoped<ICurrentUserService, CurrentUserService>();
            //services.AddScoped<IDomainEventService, DomainEventService>();
           // services.AddTransient<IIdentityService, IdentityService>();
            //services.AddTransient<IDateTime, DateTimeService>();
            //services.Configure<AdministratorsOptions>(configuration.GetSection(AdministratorsOptions.Administrators));
            //services.AddTransient<IAdministratorsProvider, AdministratorsProvider>();

            return services;
        }
    }
}