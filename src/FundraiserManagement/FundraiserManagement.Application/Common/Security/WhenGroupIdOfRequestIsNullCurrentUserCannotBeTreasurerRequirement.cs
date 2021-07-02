using FundraiserManagement.Application.Common.Interfaces.Auth;
using Microsoft.AspNetCore.Authorization;
using SharedKernel.Domain.Constants;
using SharedKernel.Infrastructure.Extensions;
using System;
using System.Threading.Tasks;

namespace FundraiserManagement.Application.Common.Security
{
    internal sealed class WhenGroupIdOfRequestIsNullCurrentUserCannotBeTreasurerRequirement : IAuthorizationRequirement
    {
    }

    internal sealed class WhenGroupIdOfRequestIsNullCurrentUserCannotBeTreasurerRequirementHandler :
        AuthorizationHandler<WhenGroupIdOfRequestIsNullCurrentUserCannotBeTreasurerRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            WhenGroupIdOfRequestIsNullCurrentUserCannotBeTreasurerRequirement requirement)
        {
            if (!(context.Resource is IGroupAuthorizationRequest request))
                throw new InvalidOperationException(context.Resource.GetGenericTypeName());


            if (request.GroupId is null && context.User.IsInRole(GroupRoles.Treasurer))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}