using Microsoft.AspNetCore.Authorization;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using static SchoolManagement.Core.SchoolAggregate.Users.User;

namespace Fundraiser.API.Authorization.SubMustMatchAdminId
{
    public sealed class SubMustMatchAdminIdHandler : AuthorizationHandler<SubMustMatchAdminIdRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SubMustMatchAdminIdRequirement requirement)
        {
            string subAsString = context.User.FindFirstValue("sub");

            if (!Guid.TryParse(subAsString, out Guid userId))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            if (userId != Admin.Id)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
