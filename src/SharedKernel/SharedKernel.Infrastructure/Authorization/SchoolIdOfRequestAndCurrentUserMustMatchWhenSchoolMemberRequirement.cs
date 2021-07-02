using Microsoft.AspNetCore.Authorization;
using SharedKernel.Infrastructure.Abstractions.Requests;
using SharedKernel.Infrastructure.Extensions;
using System;
using System.Threading.Tasks;

namespace SharedKernel.Infrastructure.Authorization
{
    public sealed class SchoolIdOfRequestAndCurrentUserMustMatchWhenSchoolMemberRequirement : IAuthorizationRequirement
    {
        public SchoolIdOfRequestAndCurrentUserMustMatchWhenSchoolMemberRequirement() { }
    }

    public sealed class SchoolIdOfRequestAndCurrentUserMustMatchWhenSchoolMember 
        : AuthorizationHandler<SchoolIdOfRequestAndCurrentUserMustMatchWhenSchoolMemberRequirement>
    {

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            SchoolIdOfRequestAndCurrentUserMustMatchWhenSchoolMemberRequirement requirement)
        {
            if (!(context.Resource is ISchoolAuthorizationRequest request))
                throw new InvalidOperationException(context.Resource.GetGenericTypeName());

            if (context.User.IsInSchoolRole() && !context.User.IsInSchool(request.SchoolId))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}