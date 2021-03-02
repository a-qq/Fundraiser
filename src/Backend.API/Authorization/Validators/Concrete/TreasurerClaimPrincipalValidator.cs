using System.Security.Claims;
using System.Threading.Tasks;
using Backend.API.Authorization.Validators.Absrtact;
using MediatR;
using SchoolManagement.Domain.SchoolAggregate.Groups;
using SchoolManagement.Domain.SchoolAggregate.Members;

namespace Backend.API.Authorization.Validators.Concrete
{
    internal sealed class TreasurerClaimPrincipalValidator : ClaimPrincipalValidatorForSchoolRolesBase,
        IClaimsPrincipalValidator
    {
        public TreasurerClaimPrincipalValidator(ISender mediator)
            : base(mediator)
        {
        }

        public override string RoleRequirement { get; } = GroupRoles.Treasurer;

        public override async Task<bool> IsValidAsync(ClaimsPrincipal principal)
        {
            if (!await base.IsValidAsync(principal))
                return false;

            if (!IsStudent(principal))
                return false;

            if (!AuthorizationData.Value.IsTreasurer)
                return false;

            return true;
        }

        private bool IsStudent(ClaimsPrincipal principal)
        {
            if (!principal.IsInRole(RoleEnum.Student.ToString()))
                return false;

            if (!AuthorizationData.HasValue || AuthorizationData.Value.Role != RoleEnum.Student)
                return false;

            return true;
        }
    }
}