////using IdentityServer4.AccessTokenValidation;

//using Microsoft.AspNetCore.Authorization;
//using Microsoft.Extensions.DependencyInjection;
//using SchoolManagement.Domain.SchoolAggregate.Members;
//using SharedKernel.Domain.Constants;
//using SharedKernel.Domain.EnumeratedEntities;
//using System.Reflection;

//namespace SchoolManagement.Infrastructure.Authorization
//{
//    public static class AuthorizationInstaller
//    {
//        public static void AddSharedKernelAuthorizationModule(this IServiceCollection services)
//        {
//            services.Scan(scan => scan
//                .FromAssemblies(Assembly.GetExecutingAssembly())
//                .AddClasses(classes => classes.AssignableTo<IAuthorizationHandler>())
//                .AsImplementedInterfaces()
//                .WithTransientLifetime());

//            services.AddAuthorization(options =>
//            {
//                options.AddPolicy("MustBeAdmin", builder =>
//                {
//                    builder.RequireRole(nameof(Administrator.RoleName));
//                    //builder.RequireClaim(JwtClaimTypes.Subject);
//                    //builder.AddAuthenticationSchemes(IdentityServerAuthenticationDefaults.AuthenticationScheme);
//                    builder.RequireAuthenticatedUser();
//                    builder.AddRequirements(new CurrentUserIdMustMatchAdminIdWhenUserIsAdminRequirement());
//                });

//                options.AddPolicy("MustBeHeadmaster", builder =>
//                {
//                    builder.RequireRole(Role.Headmaster);
//                    //builder.RequireClaim(JwtClaimTypes.Subject);
//                    //builder.RequireClaim(CustomClaimTypes.SchoolId);
//                    //builder.AddAuthenticationSchemes(IdentityServerAuthenticationDefaults.AuthenticationScheme);
//                    builder.RequireAuthenticatedUser();
//                    builder.AddRequirements(new SchoolIdOfRequestAndCurrentUserMustMatchWhenSchoolMemberRequirement());
//                });

//                options.AddPolicy("MustBeFormTutor", builder =>
//                {
//                    builder.RequireRole(Role.Teacher);
//                    //builder.RequireClaim(JwtClaimTypes.Subject);
//                    //builder.RequireClaim(CustomClaimTypes.SchoolId);
//                    //builder.RequireClaim(CustomClaimTypes.GroupId);
//                    //builder.AddAuthenticationSchemes(IdentityServerAuthenticationDefaults.AuthenticationScheme);
//                    builder.RequireAuthenticatedUser();
//                    builder.AddRequirements(new GroupIdOfRequestAndCurrentUserMustMatchWhenGroupMemberRequirement());
//                });

//                options.AddPolicy("MustBeAtLeastHeadmaster", builder =>
//                {
//                    builder.RequireRole(Role.Headmaster, Administrator.RoleName);
                  
//                    //builder.RequireClaim(JwtClaimTypes.Subject);
//                    //builder.AddAuthenticationSchemes(IdentityServerAuthenticationDefaults.AuthenticationScheme);
//                    builder.RequireAuthenticatedUser();
//                    builder.AddRequirements(
//                        new SchoolIdOfRequestAndCurrentUserMustMatchWhenSchoolMemberRequirement(),
//                        new CurrentUserIdMustMatchAdminIdWhenUserIsAdminRequirement());
//                });

//                options.AddPolicy("MustBeAtLeastFormTutor", builder =>
//                {
//                    builder.RequireRole(Role.Headmaster, Administrator.RoleName, GroupRoles.FormTutor);
//                    //builder.RequireClaim(JwtClaimTypes.Subject);
//                    //builder.AddAuthenticationSchemes(IdentityServerAuthenticationDefaults.AuthenticationScheme);
//                    builder.RequireAuthenticatedUser();
//                    builder.AddRequirements(
//                        new GroupIdOfRequestAndCurrentUserMustMatchWhenGroupMemberRequirement(),
//                        new CurrentUserIdMustMatchAdminIdWhenUserIsAdminRequirement());
//                });
//            });

//            //services.AddTransient<IClaimPrincipalValidatorsFactory, ClaimPrincipalValidatorsFactory>();
//        }
//    }
//}