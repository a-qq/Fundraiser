//using System;
//using System.Threading.Tasks;
//using Ardalis.GuardClauses;
//using Microsoft.AspNetCore.Authorization;
//using SharedKernel.Domain.Constants;
//using SharedKernel.Infrastructure.Authorization.Validators.Abstract;

//namespace SharedKernel.Infrastructure.Authorization
//{
//    /// <summary>
//    ///     Check for valid role in given order. Throws if validator for given role is not implemented.
//    /// </summary>
//    public class OneOfUserRolesMustBeValidRequirement : IAuthorizationRequirement
//    {
//        public OneOfUserRolesMustBeValidRequirement(Role schoolRole, GroupRole groupRole)
//        {
//            SchoolRole = Guard.Against.Null(schoolRole, naemof(schoolRole));
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
//            var isAuthorized = false;
//            for (var i = 0; i < requirement.Roles.Count; i++)
//            {
//                var role = requirement.Roles[i];
//                if (string.IsNullOrWhiteSpace(role))
//                    throw new ArgumentException(nameof(requirement.Roles) + $"[{i}]");

//                var validator = _claimPrincipalValidatorsFactory.GetValidatorByRequiredRole(role);
//                isAuthorized = await validator.IsValidAsync(context.User);
//                if (isAuthorized)
//                    break;
//            }

//            if (!isAuthorized)
//            {
//                context.Fail();
//                return;
//            }

//            context.Succeed(requirement);
//        }
//    }
//}