using Backend.API.Authorization.Validators.Absrtact;
using MediatR;
using SchoolManagement.Domain.SchoolAggregate.Members;

namespace Backend.API.Authorization.Validators.Concrete
{
    internal sealed class HeadmasterClaimPrincipalValidator : ClaimPrincipalValidatorForSchoolRolesBase,
        IClaimsPrincipalValidator
    {
        public HeadmasterClaimPrincipalValidator(ISender mediator)
            : base(mediator)
        {
        }

        public override string RoleRequirement { get; } = RoleEnum.Headmaster.ToString();
    }
}