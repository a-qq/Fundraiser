//using System.Threading.Tasks;
//using Ardalis.GuardClauses;
//using Microsoft.AspNetCore.Authorization;
//using SchoolManagement.Domain.SchoolAggregate.Groups;
//using SchoolManagement.Domain.SchoolAggregate.Members;
//using SchoolManagement.Infrastructure.Authorization.Validators.Abstract;
//using SharedKernel.Domain.Constants;

//namespace SchoolManagement.Infrastructure.Authorization
//{
//    /// <summary>
//    ///     Check for valid role in given order. Throws if validator for given role is not implemented.
//    /// </summary>
//    public class OneOfUserRolesMustBeValidRequirement : IAuthorizationRequirement
//    {
//        public OneOfUserRolesMustBeValidRequirement(Role schoolRole, GroupRole groupRole)
//        {
//            SchoolRole = Guard.Against.Null(schoolRole, nameof(schoolRole));
//            GroupRole = groupRole;
//        }

//        public Role SchoolRole { get; } 
//        public GroupRole GroupRole { get; }
//    }

//    internal sealed class OneOfUserRolesMustBeValidHandler : AuthorizationHandler<OneOfUserRolesMustBeValidRequirement>
//    {
//        private readonly IClaimPrincipalValidatorsFactory _claimPrincipalValidatorsFactory;

//        public OneOfUserRolesMustBeValidHandler(
//            IClaimPrincipalValidatorsFactory claimPrincipalValidatorsFactory)
//        {
//            _claimPrincipalValidatorsFactory = claimPrincipalValidatorsFactory;
//        }

//        protected override async Task HandleRequirementAsync(
//            AuthorizationHandlerContext context, OneOfUserRolesMustBeValidRequirement requirement)
//        {
//            var 
//            var validator = _claimPrincipalValidatorsFactory.GetValidatorByRequiredRole(requirement.SchoolRole);
//            isAuthorized = await validator.IsValidAsync(context.User);
//            if (isAuthorized)
//                break;
        
//        if (!isAuthorized)
//            {
//                context.Fail();
//                return;
//            }

//            context.Succeed(requirement);
//        }
//    }
//}