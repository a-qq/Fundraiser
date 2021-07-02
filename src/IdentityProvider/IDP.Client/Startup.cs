using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentValidation.AspNetCore;
using IDP.Application;
using IDP.Application.Common.Options;
using IDP.Application.IntegrationEvents.Events;
using IDP.Client.Services;
using IDP.Client.Validators;
using IDP.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharedKernel.Infrastructure;
using SharedKernel.Infrastructure.Abstractions.EventBus;
using SharedKernel.Infrastructure.Options;

namespace IDP.Client
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
            services.AddControllersWithViews()
                .AddFluentValidation(fv =>
                {
                    fv.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                });
            //services.AddOptions();
            services.AddViewModelsValidators();
            //services.AddEventBus(Configuration);
            //services.AddSharedKernel(Configuration);
            //services.AddIdpApplication(Configuration);
            //services.AddIdpInfrastructure(Environment, Configuration);

            var builder = services.AddIdentityServer(options =>
                {
                    // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
                    options.EmitStaticAudienceClaim = true;
                    //options.Discovery.CustomEntries.Add("local_api", "~/localapi");
                })
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryApiResources(Config.ApiResources)
                .AddInMemoryClients(Config.Clients);

            builder.AddProfileService<ProfileService>();
            // not recommended for production - you need to store your key material somewhere secure
            builder.AddDeveloperSigningCredential();
            // for exchanging tokens (?)
            //builder.AddExtensionGrantValidator<TokenExchangeExtensionGrantValidator>();
            //for production
            //builder.AddSigningCredential(LoadCertificateFromStore());

            //var migrationsAssembly = typeof(Startup)
            //   .GetTypeInfo().Assembly.GetName().Name;

            ////add-migration -name InitialIdentityServerConfigurationDBMigration -context ConfigurationDbContext
            //builder.AddConfigurationStore(options =>
            //{
            //    options.ConfigureDbContext = builder =>
            //        builder.UseSqlServer(connectionString,
            //        options => options.MigrationsAssembly(migrationsAssembly));
            //});

            ////add-migration -name InitialIdentityServerOperationalDBMigration -context PersistedGrantDbContext
            //builder.AddOperationalStore(options =>
            //{
            //    options.ConfigureDbContext = builder =>
            //        builder.UseSqlServer(connectionString,
            //        options => options.MigrationsAssembly(migrationsAssembly));
            //});
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new ApplicationModule(
                Configuration.GetSection(SecurityCodeOptions.SecurityCode).Get<SecurityCodeOptions>()));

            builder.RegisterModule(new MediatorModule());

            builder.RegisterModule(new DataAccessModule(
                Configuration.GetConnectionString("IdentityProviderDb")));

            builder.RegisterModule(new ProcessingModule(
                Configuration.GetSection(MailOptions.MailSettings).Get<MailOptions>(),
                Configuration.GetSection(UrlsOptions.Urls).Get<UrlsOptions>(),
                Configuration.GetSection(AdministratorsOptions.Administrators).Get<AdministratorsOptions>()));

            builder.RegisterModule(new RabbitMqEventBusModule(
                Configuration["EventBus:SubscriptionClientName"],
                int.TryParse(Configuration["EventBus:RetryCount"], out int retryCount) ? retryCount : 5,
                Configuration["EventBus:Connection"],
                Configuration["EventBus:UserName"],
                Configuration["EventBus:Password"]));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            this.AutofacContainer = app.ApplicationServices.GetAutofacRoot();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });

            ConfigureEventBus(app);
        }

        private void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            eventBus.Subscribe<FormTutorAssignedIntegrationEvent, IIntegrationEventHandler<FormTutorAssignedIntegrationEvent>>();
            eventBus.Subscribe<FormTutorDivestedIntegrationEvent, IIntegrationEventHandler<FormTutorDivestedIntegrationEvent>>();
            eventBus.Subscribe<FormTutorsDivestedIntegrationEvent, IIntegrationEventHandler<FormTutorsDivestedIntegrationEvent>>();
            eventBus.Subscribe<HeadmasterDivestedIntegrationEvent, IIntegrationEventHandler<HeadmasterDivestedIntegrationEvent>>();
            eventBus.Subscribe<HeadmasterPromotedIntegrationEvent, IIntegrationEventHandler<HeadmasterPromotedIntegrationEvent>>();
            eventBus.Subscribe<MemberArchivedIntegrationEvent, IIntegrationEventHandler<MemberArchivedIntegrationEvent>>();
            eventBus.Subscribe<MemberEnrolledIntegrationEvent, IIntegrationEventHandler<MemberEnrolledIntegrationEvent>>();
            eventBus.Subscribe<MemberExpelledIntegrationEvent, IIntegrationEventHandler<MemberExpelledIntegrationEvent>>();
            eventBus.Subscribe<MemberRestoredIntegrationEvent, IIntegrationEventHandler<MemberRestoredIntegrationEvent>>();
            eventBus.Subscribe<MembersArchivedIntegrationEvent, IIntegrationEventHandler<MembersArchivedIntegrationEvent>>();
            eventBus.Subscribe<MembersEnrolledIntegrationEvent, IIntegrationEventHandler<MembersEnrolledIntegrationEvent>>();
            eventBus.Subscribe<SchoolRemovedIntegrationEvent, IIntegrationEventHandler<SchoolRemovedIntegrationEvent>>();
            eventBus.Subscribe<StudentAssignedIntegrationEvent, IIntegrationEventHandler<StudentAssignedIntegrationEvent>>();
            eventBus.Subscribe<StudentDisenrolledIntegrationEvent, IIntegrationEventHandler<StudentDisenrolledIntegrationEvent>>();
            eventBus.Subscribe<StudentsDisenrolledIntegrationEvent, IIntegrationEventHandler<StudentsDisenrolledIntegrationEvent>>();
            eventBus.Subscribe<StudentsAssignedIntegrationEvent, IIntegrationEventHandler<StudentsAssignedIntegrationEvent>>();
            eventBus.Subscribe<TreasurerDivestedIntegrationEvent, IIntegrationEventHandler<TreasurerDivestedIntegrationEvent>>();
            eventBus.Subscribe<TreasurersDivestedIntegrationEvent, IIntegrationEventHandler<TreasurersDivestedIntegrationEvent>>();
            eventBus.Subscribe<TreasurerPromotedIntegrationEvent, IIntegrationEventHandler<TreasurerPromotedIntegrationEvent>>();
            eventBus.Subscribe<PasswordResetRequestedIntegrationEvent, IIntegrationEventHandler<PasswordResetRequestedIntegrationEvent>>();
            eventBus.Subscribe<UserRegistrationRequestedIntegrationEvent, IIntegrationEventHandler<UserRegistrationRequestedIntegrationEvent>>();
        }
    }

    static class CustomExtensionsMethods
    {
        //public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration Configuration)
        //{
        //    var subscriptionClientName = Configuration["EventBus:SubscriptionClientName"];

        //    services.AddSingleton<IEventBus, EventBusRabbitMq>(sp =>
        //    {
        //        var rabbitMqPersistentConnection = sp.GetRequiredService<IRabbitMqPersistentConnection>();
        //        var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
        //        var logger = sp.GetRequiredService<ILogger<EventBusRabbitMq>>();
        //        var eventBusSubscriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

        //        var retryCount = 5;
        //        if (!string.IsNullOrEmpty(Configuration["EventBus:RetryCount"]))
        //        {
        //            retryCount = int.Parse(Configuration["EventBus:RetryCount"]);
        //        }

        //        return new EventBusRabbitMq(rabbitMqPersistentConnection, logger, iLifetimeScope,
        //            eventBusSubscriptionsManager, subscriptionClientName, retryCount);
        //    });


        //    services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

        //    return services;
        //}


        //public static IServiceCollection AddSharedKernel(this IServiceCollection services, IConfiguration Configuration)
        //{
        //    //services.AddScoped<ISqlConnectionFactory>(x =>
        //        new SqlConnectionFactory(Configuration.GetConnectionString("DefaultConnection")));

        //    //services.AddScoped<IDomainEventService, DomainEventService>();
        //    //services.AddScoped<IMailManager, MailManager>();
        //    //services.AddTransient<IDateTime, DateTimeService>();
        //    //services.Configure<MailOptions>(Configuration.GetSection(MailOptions.MailSettings));
        //    //services.Configure<UrlsOptions>(Configuration.GetSection(UrlsOptions.Urls));
        //    //services.Configure<AdministratorsOptions>(Configuration.GetSection(AdministratorsOptions.Administrators));

        //    //services.AddTransient<Func<DbContext, IRequestManager>>(
        //    //    sp => (DbContext c) => new RequestManager(c));

        //    //services.AddTransient<Func<DbConnection, Assembly, IIntegrationEventLogService>>(
        //    //    sp => (DbConnection c, Assembly a) => new IntegrationEventLogService(c, a));

        //    //services.AddSingleton<IRabbitMqPersistentConnection>(sp =>
        //    //{
        //    //    var logger = sp.GetRequiredService<ILogger<DefaultRabbitMqPersistentConnection>>();

        //    //    var factory = new ConnectionFactory()
        //    //    {
        //    //        HostName = Configuration["EventBus:Connection"],
        //    //        DispatchConsumersAsync = true
        //    //    };

        //    //    if (!string.IsNullOrEmpty(Configuration["EventBus:UserName"]))
        //    //    {
        //    //        factory.UserName = Configuration["EventBus:UserName"];
        //    //    }

        //    //    if (!string.IsNullOrEmpty(Configuration["EventBus:Password"]))
        //    //    {
        //    //        factory.Password = Configuration["EventBus:Password"];
        //    //    }

        //    //    var retryCount = 5;
        //    //    if (!string.IsNullOrEmpty(Configuration["EventBus:RetryCount"]))
        //    //    {
        //    //        retryCount = int.Parse(Configuration["EventBus:RetryCount"]);
        //    //    }

        //    //    return new DefaultRabbitMqPersistentConnection(factory, logger, retryCount);
        //    //});

        //    //services.AddSingleton<IEventReducersManager>(sp =>
        //    //{
        //    //    var logger = sp.GetRequiredService<ILogger<EventReducersManager>>();

        //    //    var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();

        //    //    return new EventReducersManager(typeof(User).GetTypeInfo().Assembly, iLifetimeScope, logger);
        //    //});

        //    //return services;
        //}
    }
}