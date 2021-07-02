//using Microsoft.AspNetCore.Authorization;
//using SharedKernel.Infrastructure.Abstractions.Common;
//using System;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using SharedKernel.Domain.EnumeratedEntities;

//namespace SchoolManagement.Infrastructure.Authorization
//{
//    public sealed class CurrentUserIdMustMatchAdminIdWhenUserIsAdminRequirement : IAuthorizationRequirement
//    {
//        public CurrentUserIdMustMatchAdminIdWhenUserIsAdminRequirement() { }
//    }

//    public sealed class CurrentUserIdMustMatchAdminIdWhenUserIsAdminRequirementHandler : AuthorizationHandler<CurrentUserIdMustMatchAdminIdWhenUserIsAdminRequirement>
//    {
//        private readonly IAdministratorsProvider _administratorProvider;

//        public CurrentUserIdMustMatchAdminIdWhenUserIsAdminRequirementHandler(IAdministratorsProvider administratorProvider)
//        {
//            _administratorProvider = administratorProvider;
//        }
//        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CurrentUserIdMustMatchAdminIdWhenUserIsAdminRequirement whenUserIsAdminRequirement)
//        {
//            if (context.User.IsInRole(Administrator.RoleName) &&
//                (!Guid.TryParse(context.User.FindFirstValue("sub"), out Guid userId) ||
//                 !_administratorProvider.ExistById(userId))) 
//            {
//                context.Fail();
//                return Task.CompletedTask;
//            }

//            context.Succeed(whenUserIsAdminRequirement);
//            return Task.CompletedTask;
//        }
//    }
//}