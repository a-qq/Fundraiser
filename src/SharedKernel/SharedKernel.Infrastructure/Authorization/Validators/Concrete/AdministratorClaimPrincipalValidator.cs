//using System;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using SharedKernel.Domain.EnumeratedEntities;
//using SharedKernel.Infrastructure.Abstractions.Common;
//using SharedKernel.Infrastructure.Authorization.Validators.Abstract;

////namespace SharedKernel.Infrastructure.Authorization.Validators.Concrete
////{
////    internal sealed class AdministratorClaimPrincipalValidator : ClaimPrincipalValidatorBase, IClaimsPrincipalValidator
////    {
////        private readonly IAdministratorsProvider _adminProvider;

////        public AdministratorClaimPrincipalValidator(IAdministratorsProvider adminProvider)
////        {
////            _adminProvider = adminProvider;
////        }

////        public string RoleRequirement { get; } = Administrator.RoleName;

////        public Task<bool> IsValidAsync(ClaimsPrincipal principal)
////        {
////            if (!HasValidUserId(principal, out var userId))
////                return Task.FromResult(false);

////            if (!IsAdmin(principal, userId))
////                return Task.FromResult(false);

////            return Task.FromResult(true);
////        }

////        private bool IsAdmin(ClaimsPrincipal principal, Guid userId)
////        {
////            return principal.IsInRole(RoleRequirement) && _adminProvider.ExistById(userId);
////        }
////    }
////}