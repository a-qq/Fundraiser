using FundraiserManagement.Application.Common.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Domain.Constants;
using SharedKernel.Domain.EnumeratedEntities;
using SharedKernel.Infrastructure.Authorization;
using System.Reflection;

namespace FundraiserManagement.Application
{
    public static class AuthorizationInstaller
    {
        public static void AddFundraiserManagementAuthorizationModule(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
             {
                 options.AddPolicy(PolicyNames.CanCreateFundraiser, builder =>
                 {
                     builder.RequireRole(SchoolRole.Headmaster.ToString(), Administrator.RoleName, GroupRoles.FormTutor, GroupRoles.Treasurer);
                     builder.RequireAuthenticatedUser();
                     builder.AddRequirements(
                         new MustBeEligibleToCreateFundraiserRequirement(),
                         new GroupIdOfRequestAndCurrentUserMustMatchWhenGroupMemberAndNotNullRequirement(),
                         new CurrentUserIdMustMatchAdminIdWhenUserIsAdminRequirement(),
                         new SchoolIdOfRequestAndCurrentUserMustMatchWhenSchoolMemberRequirement());
                 });

                 options.AddPolicy(PolicyNames.CanModifyFundraiser, builder =>
                 {
                     builder.RequireRole(SchoolRole.Headmaster.ToString(), Administrator.RoleName, GroupRoles.FormTutor, GroupRoles.Treasurer);
                     builder.RequireAuthenticatedUser();
                     builder.AddRequirements(
                         new SchoolIdOfRequestAndCurrentUserMustMatchWhenSchoolMemberRequirement(),
                         new CurrentUserIdMustMatchAdminIdWhenUserIsAdminRequirement(),
                         new MustBeEligibleToCreateFundraiserRequirement(),
                         new MustBeEligibleToModifyFundraiserRequirement());
                 });

                 options.AddPolicy(PolicyNames.CanManageFundraiser, builder =>
                 {
                     builder.RequireRole(SchoolRole.Headmaster.ToString(), Administrator.RoleName, GroupRoles.FormTutor, GroupRoles.Treasurer);
                     builder.RequireAuthenticatedUser();
                     builder.AddRequirements(
                         new SchoolIdOfRequestAndCurrentUserMustMatchWhenSchoolMemberRequirement(),
                         new CurrentUserIdMustMatchAdminIdWhenUserIsAdminRequirement(),
                         new MustBeEligibleToModifyFundraiserRequirement());
                 });
             });

            services.Scan(scan => scan
                .FromAssemblies(Assembly.GetExecutingAssembly())
                .AddClasses(classes => classes.AssignableTo<IAuthorizationHandler>())
                .AsImplementedInterfaces()
                .WithTransientLifetime());
        }
    }
}