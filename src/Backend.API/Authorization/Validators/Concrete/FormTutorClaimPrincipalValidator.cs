using Backend.API.Authorization.Validators.Absrtact;
using MediatR;
using SchoolManagement.Domain.SchoolAggregate.Groups;
using SchoolManagement.Domain.SchoolAggregate.Members;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Backend.API.Authorization.Validators.Concrete
{
    internal sealed class FormTutorClaimPrincipalValidator : ClaimPrincipalValidatorForSchoolRolesBase, IClaimsPrincipalValidator
    {
        public override string RoleRequirement { get; } = GroupRoles.FormTutor;

        public FormTutorClaimPrincipalValidator(ISender mediator)
            : base(mediator) { }

        public override async Task<bool> IsValidAsync(ClaimsPrincipal principal)
        {
            if (!await base.IsValidAsync(principal))
                return false;

            if (!IsTeacher(principal))
                return false;

            return true;
        }

        private bool IsTeacher(ClaimsPrincipal principal)
        {
            if (!principal.IsInRole(RoleEnum.Teacher.ToString()))
                return false;

            if (AuthorizationData.HasValue && AuthorizationData.Value.Role != RoleEnum.Teacher)
                return false;

            return true;
        }
    }
}
