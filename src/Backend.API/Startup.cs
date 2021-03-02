using System.Text.Json.Serialization;
using Backend.API.Authorization;
using Backend.API.Extensions;
using Backend.API.Filters;
using Backend.API.IdentityServer;
using Backend.API.Middleware;
using Backend.API.Services;
using Backend.API.Validators;
using FluentValidation.AspNetCore;
using IdentityServer4.AccessTokenValidation;
using IDP.Application;
using IDP.Application.Users;
using IDP.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SchoolManagement.Application;
using SchoolManagement.Infrastructure;
using SharedKernel.Infrastructure;
using SharedKernel.Infrastructure.Interfaces;
using SharedKernel.Infrastructure.Utils;

namespace Backend.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Env = env;
            Configuration = configuration;
        }

        public IWebHostEnvironment Env { get; }
        public IConfiguration Configuration { get; }

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
            services.AddOptions();
            services.AddSchoolManagementApplication();
            services.AddSchoolManagementInfrastructure(Configuration, Env);
            services.AddIdpApplication(Configuration);
            services.AddIdpInfrastructure(Env, Configuration);
            services.AddViewModelsValidators();
            services.AddSharedKernelInfrastructure(Configuration);
            services.AddAuthorizationModule();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddHttpContextAccessor();
            services.AddSwaggerConfiguration();

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = EnvironmentVariables.IdpUrl;
                    options.ApiName = "fundraiserapi";
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
                    options.OAuthAppName("Backend.API - Swagger");
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
                id.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });
            });
        }
    }
}