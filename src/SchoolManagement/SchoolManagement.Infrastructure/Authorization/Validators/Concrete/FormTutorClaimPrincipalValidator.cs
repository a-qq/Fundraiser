//using System.Security.Claims;
//using System.Threading.Tasks;
//using MediatR;
//using SchoolManagement.Domain.SchoolAggregate.Groups;
//using SchoolManagement.Domain.SchoolAggregate.Members;
//using SchoolManagement.Infrastructure.Authorization.Validators.Abstract;
//using SharedKernel.Domain.Constants;

//namespace SchoolManagement.Infrastructure.Authorization.Validators.Concrete
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