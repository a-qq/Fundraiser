using Fundraiser.SharedKernel.Utils;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

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

            if (Administrator.FromId(userId) == null)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
