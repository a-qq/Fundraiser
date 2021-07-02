//using IdentityModel;
//using Microsoft.AspNetCore.Authorization;
//using SchoolManagement.Domain.SchoolAggregate.Members;
//using SharedKernel.Infrastructure.Abstractions.Requests;
//using SharedKernel.Infrastructure.Extensions;
//using System;
//using System.Linq;
//using System.Threading.Tasks;

//namespace SchoolManagement.Infrastructure.Authorization
//{
//    public sealed class SchoolIdOfRequestAndCurrentUserMustMatchWhenSchoolMemberRequirement : IAuthorizationRequirement
//    {
//        public SchoolIdOfRequestAndCurrentUserMustMatchWhenSchoolMemberRequirement() { }
//    }

//    public sealed class SchoolIdOfRequestAndCurrentUserMustMatchWhenSchoolMember 
//        : AuthorizationHandler<SchoolIdOfRequestAndCurrentUserMustMatchWhenSchoolMemberRequirement>
//    {

//        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
//            SchoolIdOfRequestAndCurrentUserMustMatchWhenSchoolMemberRequirement requirement)
//        {
//            if (!(context.Resource is ISchoolAuthorizationRequest request))
//                throw new InvalidOperationException(context.Resource.GetGenericTypeName());

//            if (context.User.FindAll(JwtClaimTypes.Role).Any(r => IsSchoolRole(r.Value)) && !context.User.IsInSchool(request.SchoolId))
//            {
//                context.Fail();
//                return Task.CompletedTask;
//            }

//            context.Succeed(requirement);
//            return Task.CompletedTask;
//        }

//        private bool IsSchoolRole(string role)
//            => Role.ValidateAndConvert(role).IsSuccess;
//    }
//}