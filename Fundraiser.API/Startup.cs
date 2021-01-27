using AutoMapper;
using FluentValidation.AspNetCore;
using Fundraiser.API.Authorization;
using Fundraiser.API.Authorization.SubMustMatchAdminId;
using Fundraiser.API.Authorization.UserMustBeSchoolMember;
using Fundraiser.API.Extensions;
using Fundraiser.API.Filters;
using Fundraiser.API.Middleware;
using Fundraiser.API.Validators;
using Fundraiser.IDP;
using Fundraiser.SharedKernel.Configuration;
using Fundraiser.SharedKernel.Settings;
using Fundraiser.SharedKernel.Utils;
using IdentityServer4.AccessTokenValidation;
using IDP.Infrastructure.Configuration;
using IDP.Infrastructure.IntegrationHandlers;
using IDP.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SchoolManagement.Core.SchoolAggregate.Members;
using SchoolManagement.Data.Configuration;
using SchoolManagement.Data.IntegrationHandlers;
using SchoolManagement.Data.Profiles;
using SchoolManagement.Data.Schools;
using System;
using System.Text.Json.Serialization;

namespace Fundraiser.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Env = env;
            Configuration = configuration;
            _connectionString = Configuration.GetConnectionString("SqlExpress");
        }
        public IWebHostEnvironment Env { get; }
        public IConfiguration Configuration { get; }

        private readonly string _connectionString;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews(options =>
            {
                options.Filters.Add<ValidationFilter>();

            }).ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;

            }).AddFluentValidation(fv =>
            {
                fv.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                fv.RegisterValidatorsFromAssemblyContaining<IMarkerValidator>();
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                //options.JsonSerializerOptions.IgnoreNullValues = true;
            });

            services.AddMediatR(cfg => cfg.AsScoped(), typeof(MemberDTO), typeof(IHandler), typeof(IHandlerIDP));
            services.AddAutoMapper(typeof(SchoolProfile));
            services.AddSchoolManagementInfrastructureModule(Env, _connectionString);
            services.AddIDPInfrastructureModule(Env, _connectionString);
            services.AddSharedKernelModule(_connectionString);
            services.AddAuthorizationHandlers();

            services.AddSwaggerConfiguration();

            services.AddOptions();

            var mailSettingsSection = Configuration.GetSection(nameof(MailSettings));
            services.Configure<MailSettings>(mailSettingsSection);

            var frontendSettingsSection = Configuration.GetSection(nameof(FrontendSettings));
            services.Configure<FrontendSettings>(frontendSettingsSection);

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = Environment.GetEnvironmentVariable("FrontendSettings__IDPUrl");
                    options.ApiName = "fundraiserapi";
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("MustBeAdmin", builder =>
                {
                    builder.RequireRole(nameof(Administrator));
                    builder.RequireClaim("sub");
                    builder.AddAuthenticationSchemes(IdentityServerAuthenticationDefaults.AuthenticationScheme);
                    builder.RequireAuthenticatedUser();
                    builder.AddRequirements(new SubMustMatchAdminIdRequirement());
                });

                options.AddPolicy("MustBeHeadmaster", builder =>
                {
                    builder.RequireRole(Role.Headmaster);
                    builder.RequireClaim("sub");
                    builder.RequireClaim("school_id");
                    builder.RequireAuthenticatedUser();
                    builder.AddAuthenticationSchemes(IdentityServerAuthenticationDefaults.AuthenticationScheme);
                    builder.AddRequirements(new UserMustBeSchoolMemberRequirement());
                });
            });

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

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {

            //if (Env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //else 

            app.Map("/api", api =>
            {
                api.UseMiddleware<ExceptionHandler>();
                app.UseHttpsRedirection();
                app.UseStaticFiles();
                api.UseSwagger();
                api.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/api/swagger/v1/swagger.json", "Fundraiser v1");
                    options.OAuthClientId("fundraiserapi_swagger");
                    options.OAuthAppName("Fundraiser API - Swagger");
                    options.OAuthUsePkce();
                });
                api.UseRouting();
                api.UseAuthentication();
                api.UseAuthorization();
                api.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();

                    //var pipeline = endpoints.CreateApplicationBuilder().Build();
                    //var oidcAuthAttr = new AuthorizeAttribute { AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme };
                    //endpoints
                    //    .Map("/api/swagger/{documentName}/swagger.json", pipeline)
                    //    .RequireAuthorization(oidcAuthAttr);
                    //endpoints
                    //    .Map("/api/swagger/index.html", pipeline)
                    //    .RequireAuthorization(oidcAuthAttr);
                });
            });

            app.Map("/openid", id =>
            {
                id.UseMiddleware<ExceptionHandler>();
                id.UseHttpsRedirection();
                id.UseStaticFiles();
                id.UseRouting();
                id.UseIdentityServer();
#pragma warning disable ASP0001 // Authorization middleware is incorrectly configured.
                id.UseAuthorization();
#pragma warning restore ASP0001 // Authorization middleware is incorrectly configured.
                id.UseEndpoints(endpoints =>
                {
                    endpoints.MapDefaultControllerRoute();
                });
            });
        }
    }
}
