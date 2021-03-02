﻿using System.Reflection;
using Backend.API.Authorization.Validators.Absrtact;
using Backend.API.Authorization.Validators.Concrete;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using SchoolManagement.Domain.SchoolAggregate.Groups;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Domain.EnumeratedEntities;

namespace Backend.API.Authorization
{
    public static class DependencyInjection
    {
        public static void AddAuthorizationModule(this IServiceCollection services)
        {
            services.Scan(scan => scan
                .FromAssemblies(Assembly.GetExecutingAssembly())
                .AddClasses(classes => classes.AssignableTo<IAuthorizationHandler>())
                .AsImplementedInterfaces()
                .WithTransientLifetime());

            services.AddAuthorization(options =>
            {
                options.AddPolicy("MustBeAdmin", builder =>
                {
                    //builder.RequireRole(nameof(Administrator));
                    //builder.RequireClaim(JwtClaimTypes.Subject);
                    builder.AddAuthenticationSchemes(IdentityServerAuthenticationDefaults.AuthenticationScheme);
                    builder.RequireAuthenticatedUser();
                    builder.AddRequirements(new OneOfUserRolesMustBeValidRequirement(Administrator.RoleName));
                });

                options.AddPolicy("MustBeHeadmaster", builder =>
                {
                    //builder.RequireRole(Role.Headmaster);
                    //builder.RequireClaim(JwtClaimTypes.Subject);
                    //builder.RequireClaim(CustomClaimTypes.SchoolId);
                    builder.AddAuthenticationSchemes(IdentityServerAuthenticationDefaults.AuthenticationScheme);
                    builder.RequireAuthenticatedUser();
                    builder.AddRequirements(new OneOfUserRolesMustBeValidRequirement(Role.Headmaster));
                });

                options.AddPolicy("MustBeFormTutor", builder =>
                {
                    //builder.RequireRole(Role.Teacher);
                    //builder.RequireClaim(JwtClaimTypes.Subject);
                    //builder.RequireClaim(CustomClaimTypes.SchoolId);
                    //builder.RequireClaim(CustomClaimTypes.GroupId);
                    builder.AddAuthenticationSchemes(IdentityServerAuthenticationDefaults.AuthenticationScheme);
                    builder.RequireAuthenticatedUser();
                    builder.AddRequirements(new OneOfUserRolesMustBeValidRequirement(GroupRoles.FormTutor));
                });

                options.AddPolicy("MustBeAtLeastHeadmaster", builder =>
                {
                    //builder.RequireRole(Role.Headmaster, nameof(Administrator));
                    //builder.RequireClaim(JwtClaimTypes.Subject);
                    builder.AddAuthenticationSchemes(IdentityServerAuthenticationDefaults.AuthenticationScheme);
                    builder.RequireAuthenticatedUser();
                    builder.AddRequirements(
                        new OneOfUserRolesMustBeValidRequirement(Administrator.RoleName, Role.Headmaster));
                });

                options.AddPolicy("MustBeAtLeastFormTutor", builder =>
                {
                    //builder.RequireRole(Role.Headmaster, nameof(Administrator), GroupRoles.FormTutor);
                    //builder.RequireClaim(JwtClaimTypes.Subject);
                    builder.AddAuthenticationSchemes(IdentityServerAuthenticationDefaults.AuthenticationScheme);
                    builder.RequireAuthenticatedUser();
                    builder.AddRequirements(new OneOfUserRolesMustBeValidRequirement(Administrator.RoleName,
                        Role.Headmaster, GroupRoles.FormTutor));
                });
            });

            services.AddTransient<IClaimPrincipalValidatorsFactory, ClaimPrincipalValidatorsFactory>();
        }
    }
}