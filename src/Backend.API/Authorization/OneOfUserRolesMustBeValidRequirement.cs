using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.API.Authorization.Validators.Absrtact;
using Microsoft.AspNetCore.Authorization;

namespace Backend.API.Authorization
{
    /// <summary>
    ///     Check for valid role in given order. Throws if validator for given role is not implemented.
    /// </summary>
    public class OneOfUserRolesMustBeValidRequirement : IAuthorizationRequirement
    {
        public OneOfUserRolesMustBeValidRequirement(string role, params string[] roles)
        {
            if (string.IsNullOrWhiteSpace(role))
                throw new ArgumentNullException(nameof(role));

            var list = new List<string> {role};
            if (roles.Length > 0)
                list.AddRange(roles.Distinct().Except(list));

            Roles = list;
        }

        public IReadOnlyList<string> Roles { get; }
    }

    internal sealed class OneOfUserRolesMustBeValidHandler : AuthorizationHandler<OneOfUserRolesMustBeValidRequirement>
    {
        private readonly IClaimPrincipalValidatorsFactory _claimPrincipalValidatorsFactory;

        public OneOfUserRolesMustBeValidHandler(
            IClaimPrincipalValidatorsFactory claimPrincipalValidatorsFactory)
        {
            _claimPrincipalValidatorsFactory = claimPrincipalValidatorsFactory;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context, OneOfUserRolesMustBeValidRequirement requirement)
        {
            var isAuthorized = false;
            for (var i = 0; i < requirement.Roles.Count; i++)
            {
                var role = requirement.Roles[i];
                if (string.IsNullOrWhiteSpace(role))
                    throw new ArgumentException(nameof(requirement.Roles) + $"[{i}]");

                var validator = _claimPrincipalValidatorsFactory.GetValidatorByRequiredRole(role);
                isAuthorized = await validator.IsValidAsync(context.User);
                if (isAuthorized)
                    break;
            }

            if (!isAuthorized)
            {
                context.Fail();
                return;
            }

            context.Succeed(requirement);
        }
    }
}