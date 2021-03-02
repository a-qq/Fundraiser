using Backend.API.Authorization.Validators.Absrtact;
using SharedKernel.Domain.EnumeratedEntities;
using SharedKernel.Infrastructure.Interfaces;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Backend.API.Authorization.Validators.Concrete
{
    internal sealed class AdministratorClaimPrincipalValidator : ClaimPrincipalValidatorBase, IClaimsPrincipalValidator
    {
        private readonly IAdministratorsProvider _adminProvider;
        public string RoleRequirement { get; } = Administrator.RoleName;

        public AdministratorClaimPrincipalValidator(IAdministratorsProvider adminProvider)
            : base()
        {
            _adminProvider = adminProvider;
        }

        public Task<bool> IsValidAsync(ClaimsPrincipal principal)
        {
            if (!HasValidUserId(principal, out Guid userId))
                return Task.FromResult(false);

            if (!IsAdmin(principal, userId))
                return Task.FromResult(false);

            return Task.FromResult(true);
        }
        private bool IsAdmin(ClaimsPrincipal principal, Guid userId)
            => principal.IsInRole(RoleRequirement) && _adminProvider.ExistById(userId);
    }
}
