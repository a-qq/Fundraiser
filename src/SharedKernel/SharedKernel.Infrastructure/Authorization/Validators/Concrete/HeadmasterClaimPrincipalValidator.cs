//using MediatR;
//using SharedKernel.Infrastructure.Authorization.Validators.Abstract;

//namespace SharedKernel.Infrastructure.Authorization.Validators.Concrete
//{
//    internal sealed class HeadmasterClaimPrincipalValidator : ClaimPrincipalValidatorForSchoolRolesBase,
//        IClaimsPrincipalValidator
//    {
//        public HeadmasterClaimPrincipalValidator(ISender mediator)
//            : base(mediator)
//        {
//        }

//        public override string RoleRequirement { get; } = SchoolRole.Headmaster.ToString();
//    }
//}