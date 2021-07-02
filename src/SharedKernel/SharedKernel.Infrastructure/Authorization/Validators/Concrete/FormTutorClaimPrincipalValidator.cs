//using System.Security.Claims;
//using System.Threading.Tasks;
//using MediatR;
//using SharedKernel.Infrastructure.Authorization.Validators.Abstract;

//namespace SharedKernel.Infrastructure.Authorization.Validators.Concrete
//{
//    internal sealed class FormTutorClaimPrincipalValidator : ClaimPrincipalValidatorForSchoolRolesBase,
//        IClaimsPrincipalValidator
//    {
//        public FormTutorClaimPrincipalValidator(ISender mediator)
//            : base(mediator)
//        {
//        }

//        public override string RoleRequirement { get; } = GroupRoles.FormTutor;

//        public override async Task<bool> IsValidAsync(ClaimsPrincipal principal)
//        {
//            if (!await base.IsValidAsync(principal))
//                return false;

//            if (!IsTeacher(principal))
//                return false;

//            return true;
//        }

//        private bool IsTeacher(ClaimsPrincipal principal)
//        {
//            if (!principal.IsInRole(SchoolRole.Teacher.ToString()))
//                return false;

//            if (AuthorizationData.HasValue && AuthorizationData.Value.Role != SchoolRole.Teacher)
//                return false;

//            return true;
//        }
//    }
//}