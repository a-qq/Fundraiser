//using System.Security.Claims;
//using System.Threading.Tasks;
//using MediatR;
//using SharedKernel.Infrastructure.Authorization.Validators.Abstract;

//namespace SharedKernel.Infrastructure.Authorization.Validators.Concrete
//{
//    internal sealed class TreasurerClaimPrincipalValidator : ClaimPrincipalValidatorForSchoolRolesBase,
//        IClaimsPrincipalValidator
//    {
//        public TreasurerClaimPrincipalValidator(ISender mediator)
//            : base(mediator)
//        {
//        }

//        public override string RoleRequirement { get; } = GroupRoles.Treasurer;

//        public override async Task<bool> IsValidAsync(ClaimsPrincipal principal)
//        {
//            if (!await base.IsValidAsync(principal))
//                return false;

//            if (!IsStudent(principal))
//                return false;

//            if (!AuthorizationData.Value.IsTreasurer)
//                return false;

//            return true;
//        }

//        private bool IsStudent(ClaimsPrincipal principal)
//        {
//            if (!principal.IsInRole(SchoolRole.Student.ToString()))
//                return false;

//            if (!AuthorizationData.HasValue || AuthorizationData.Value.Role != SchoolRole.Student)
//                return false;

//            return true;
//        }
//    }
//}