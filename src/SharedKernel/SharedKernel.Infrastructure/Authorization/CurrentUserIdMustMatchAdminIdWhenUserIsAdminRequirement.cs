using Microsoft.AspNetCore.Authorization;
using SharedKernel.Domain.EnumeratedEntities;
using SharedKernel.Infrastructure.Abstractions.Common;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SharedKernel.Infrastructure.Authorization
{
    public sealed class CurrentUserIdMustMatchAdminIdWhenUserIsAdminRequirement : IAuthorizationRequirement
    {
        public CurrentUserIdMustMatchAdminIdWhenUserIsAdminRequirement() { }
    }

    public sealed class CurrentUserIdMustMatchAdminIdWhenUserIsAdminRequirementHandler : AuthorizationHandler<CurrentUserIdMustMatchAdminIdWhenUserIsAdminRequirement>
    {
        private readonly IAdministratorsProvider _administratorProvider;

        public CurrentUserIdMustMatchAdminIdWhenUserIsAdminRequirementHandler(IAdministratorsProvider administratorProvider)
        {
            _administratorProvider = administratorProvider;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CurrentUserIdMustMatchAdminIdWhenUserIsAdminRequirement requirement)
        {
            if (context.User.IsInRole(Administrator.RoleName) &&
                (!Guid.TryParse(context.User.FindFirstValue("sub"), out Guid userId) ||
                 !_administratorProvider.ExistById(userId))) 
            {
                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}