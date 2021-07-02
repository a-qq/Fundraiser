//using System;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using SchoolManagement.Infrastructure.Authorization.Validators.Abstract;
//using SharedKernel.Domain.EnumeratedEntities;
//using SharedKernel.Infrastructure.Abstractions.Common;

//namespace SchoolManagement.Infrastructure.Authorization.Validators.Concrete
//{
//    internal sealed class AdministratorClaimPrincipalValidator : ClaimPrincipalValidatorBase, IClaimsPrincipalValidator
//    {
//        private readonly IAdministratorsProvider _adminProvider;

//        public AdministratorClaimPrincipalValidator(IAdministratorsProvider adminProvider)
//        {
//            _adminProvider = adminProvider;
//        }

//        public string RoleRequirement { get; } = Administrator.RoleName;

//        public Task<bool> IsValidAsync(ClaimsPrincipal principal)
//        {
//            if (!HasValidUserId(principal, out var userId))
//                return Task.FromResult(false);

//            if (!IsAdmin(principal, userId))
//                return Task.FromResult(false);

//            return Task.FromResult(true);
//        }

//        private bool IsAdmin(ClaimsPrincipal principal, Guid userId)
//        {
//            return principal.IsInRole(RoleRequirement) && _adminProvider.ExistById(userId);
//        }
//    }
//}