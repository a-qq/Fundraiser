using Microsoft.AspNetCore.Authorization;
using SharedKernel.Domain.Constants;
using SharedKernel.Infrastructure.Abstractions.Requests;
using SharedKernel.Infrastructure.Extensions;
using System;
using System.Threading.Tasks;

namespace SharedKernel.Infrastructure.Authorization
{
    public sealed class GroupIdOfRequestAndCurrentUserMustMatchWhenGroupMemberRequirement : IAuthorizationRequirement
    {
        public GroupIdOfRequestAndCurrentUserMustMatchWhenGroupMemberRequirement() { }
    }

    public sealed class GroupIdOfRequestAndCurrentUserMustMatchWhenGroupMemberRequirementHandler
        : AuthorizationHandler<GroupIdOfRequestAndCurrentUserMustMatchWhenGroupMemberRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            GroupIdOfRequestAndCurrentUserMustMatchWhenGroupMemberRequirement requirement)
        {
            if (!(context.Resource is IGroupAuthorizationRequest request))
                throw new InvalidOperationException(context.Resource.GetGenericTypeName());

            if (!context.User.IsInRole(SchoolRole.Headmaster.ToString()) &&
                context.User.IsInGroupRole() && !context.User.IsInGroup(request.GroupId))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}