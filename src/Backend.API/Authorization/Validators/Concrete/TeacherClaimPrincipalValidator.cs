using Backend.API.Authorization.Validators.Absrtact;
using MediatR;
using SchoolManagement.Domain.SchoolAggregate.Members;

namespace Backend.API.Authorization.Validators.Concrete
{
    internal sealed class TeacherClaimPrincipalValidator : ClaimPrincipalValidatorForSchoolRolesBase,
        IClaimsPrincipalValidator
    {
        public TeacherClaimPrincipalValidator(ISender mediator)
            : base(mediator)
        {
        }

        public override string RoleRequirement { get; } = RoleEnum.Teacher.ToString();
    }
}