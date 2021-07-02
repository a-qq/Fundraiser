using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Domain.Constants;
using SharedKernel.Domain.EnumeratedEntities;
using SharedKernel.Infrastructure.Authorization;
using SharedKernel.Infrastructure.Utils;
using System.Reflection;

namespace SharedKernel.Infrastructure
{
    public static class AuthorizationInstaller
    {
        public static void AddSharedKernelAuthorizationModule(this IServiceCollection services)
        {
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = EnvironmentVariables.IdpUrl;
                    options.ApiName = "fundraiserapi";
                    //options.ApiSecret = EnvironmentVariables.JwtSecret;
                });

            services.Scan(scan => scan
                .FromAssemblies(Assembly.GetExecutingAssembly())
                .AddClasses(classes => classes.AssignableTo<IAuthorizationHandler>())
                .AsImplementedInterfaces()
                .WithTransientLifetime());

            services.AddAuthorization(options =>
            {
                options.AddPolicy(PolicyNames.MustBeAdmin, builder =>
                {
                    builder.RequireRole(Administrator.RoleName);
                    builder.RequireAuthenticatedUser();
                    builder.AddRequirements(new CurrentUserIdMustMatchAdminIdWhenUserIsAdminRequirement());
                });

                options.AddPolicy(PolicyNames.MustBeHeadmaster, builder =>
                {
                    builder.RequireRole(SchoolRole.Headmaster.ToString());
                    builder.RequireAuthenticatedUser();
                    builder.AddRequirements(new SchoolIdOfRequestAndCurrentUserMustMatchWhenSchoolMemberRequirement());
                });

                options.AddPolicy(PolicyNames.MustBeFormTutor, builder =>
                {
                    builder.RequireRole(GroupRole.FormTutor.ToString());
                    builder.RequireAuthenticatedUser();
                    builder.AddRequirements(
                        new GroupIdOfRequestAndCurrentUserMustMatchWhenGroupMemberRequirement(),
                        new SchoolIdOfRequestAndCurrentUserMustMatchWhenSchoolMemberRequirement());
                });

                options.AddPolicy(PolicyNames.MustBeAtLeastHeadmaster, builder =>
                {
                    builder.RequireRole(SchoolRole.Headmaster.ToString(), Administrator.RoleName);
                    builder.RequireAuthenticatedUser();
                    builder.AddRequirements(
                        new SchoolIdOfRequestAndCurrentUserMustMatchWhenSchoolMemberRequirement(),
                        new CurrentUserIdMustMatchAdminIdWhenUserIsAdminRequirement());
                });

                options.AddPolicy(PolicyNames.MustBeAtLeastFormTutor, builder =>
                {
                    builder.RequireRole(SchoolRole.Headmaster.ToString(), Administrator.RoleName, GroupRoles.FormTutor);
                    builder.RequireAuthenticatedUser();
                    builder.AddRequirements(
                        new GroupIdOfRequestAndCurrentUserMustMatchWhenGroupMemberRequirement(),
                        new CurrentUserIdMustMatchAdminIdWhenUserIsAdminRequirement(),
                        new SchoolIdOfRequestAndCurrentUserMustMatchWhenSchoolMemberRequirement());
                });

                options.AddPolicy(PolicyNames.MustBeAtLeastTreasurer, builder =>
                {
                    builder.RequireRole(SchoolRole.Headmaster.ToString(), Administrator.RoleName, GroupRoles.FormTutor, GroupRoles.Treasurer);
                    builder.RequireAuthenticatedUser();
                    builder.AddRequirements(
                        new GroupIdOfRequestAndCurrentUserMustMatchWhenGroupMemberRequirement(),
                        new CurrentUserIdMustMatchAdminIdWhenUserIsAdminRequirement(),
                        new SchoolIdOfRequestAndCurrentUserMustMatchWhenSchoolMemberRequirement());
                });
            });
        }
    }
}