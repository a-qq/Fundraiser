//using MediatR;
//using SharedKernel.Infrastructure.Authorization.Validators.Abstract;

//namespace SharedKernel.Infrastructure.Authorization.Validators.Concrete
//{
//    internal sealed class StudentClaimPrincipalValidator : ClaimPrincipalValidatorForSchoolRolesBase,
//        IClaimsPrincipalValidator
//    {
//        public StudentClaimPrincipalValidator(ISender mediator)
//            : base(mediator)
//        {
//        }

//        public override string RoleRequirement { get; } = SchoolRole.Student.ToString();
//    }
//}