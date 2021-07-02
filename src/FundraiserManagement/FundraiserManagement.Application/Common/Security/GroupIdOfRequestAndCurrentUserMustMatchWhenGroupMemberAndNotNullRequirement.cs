using FundraiserManagement.Application.Common.Interfaces.Auth;
using Microsoft.AspNetCore.Authorization;
using SharedKernel.Domain.Constants;
using SharedKernel.Domain.EnumeratedEntities;
using SharedKernel.Infrastructure.Extensions;
using System;
using System.Threading.Tasks;

namespace FundraiserManagement.Application.Common.Security
{
    internal sealed class GroupIdOfRequestAndCurrentUserMustMatchWhenGroupMemberAndNotNullRequirement : IAuthorizationRequirement
    {
    }

    internal sealed class GroupIdOfRequestAndCurrentUserMustMatchWhenGroupMemberAndNotNullRequirementHandler
        : AuthorizationHandler<GroupIdOfRequestAndCurrentUserMustMatchWhenGroupMemberAndNotNullRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            GroupIdOfRequestAndCurrentUserMustMatchWhenGroupMemberAndNotNullRequirement requirement)
        {
            if (!(context.Resource is IGroupAuthorizationRequest request))
                throw new InvalidOperationException(context.Resource.GetGenericTypeName());

            if (request.GroupId.HasValue &&
                !context.User.IsInRole(Administrator.RoleName) &&
                !context.User.IsInRole(SchoolRole.Headmaster.ToString()) &&
                context.User.IsInGroupRole() &&
                !context.User.IsInGroup(request.GroupId.Value))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}