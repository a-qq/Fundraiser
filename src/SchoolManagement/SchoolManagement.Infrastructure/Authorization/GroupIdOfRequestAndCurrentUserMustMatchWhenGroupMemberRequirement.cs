//using IdentityModel;
//using Microsoft.AspNetCore.Authorization;
//using SharedKernel.Domain.Constants;
//using SharedKernel.Infrastructure.Abstractions.Common;
//using SharedKernel.Infrastructure.Extensions;
//using System;
//using System.Linq;
//using System.Threading.Tasks;

//namespace SchoolManagement.Infrastructure.Authorization
//{
//    public sealed class GroupIdOfRequestAndCurrentUserMustMatchWhenGroupMemberRequirement : IAuthorizationRequirement
//    {
//        public GroupIdOfRequestAndCurrentUserMustMatchWhenGroupMemberRequirement() { }
//    }

//    public sealed class GroupIdOfRequestAndCurrentUserMustMatchWhenGroupMemberRequirementHandler
//        : AuthorizationHandler<GroupIdOfRequestAndCurrentUserMustMatchWhenGroupMemberRequirement>
//    {
//        private readonly IIdentityService _identityService;

//        public GroupIdOfRequestAndCurrentUserMustMatchWhenGroupMemberRequirementHandler(
//            IIdentityService identityService)
//        {
//            _identityService = identityService;
//        }

//        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
//            GroupIdOfRequestAndCurrentUserMustMatchWhenGroupMemberRequirement requirement)
//        {
//            if (!(context.Resource is IGroupAuthorizationRequest request))
//                throw new InvalidOperationException(context.Resource.GetGenericTypeName());

//            if (!await _identityService.AuthorizeAsync(context.User,
//                    new SchoolIdOfRequestAndCurrentUserMustMatchWhenSchoolMemberRequirement(), request) ||
//                context.User.FindAll(JwtClaimTypes.Role).Any(r => IsInGroupRole(r.Value)) && !context.User.IsInGroup(request.GroupId))
//            {
//                context.Fail();
//                return;
//            }

//            context.Succeed(requirement);
//        }

//        private bool IsInGroupRole(string role)
//        {
//            return Enum.TryParse(typeof(GroupRole), role, true, out _);
//        }
//    }
//}