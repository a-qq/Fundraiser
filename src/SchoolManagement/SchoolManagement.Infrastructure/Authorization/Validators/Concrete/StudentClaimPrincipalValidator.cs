//using MediatR;
//using SchoolManagement.Domain.SchoolAggregate.Members;
//using SchoolManagement.Infrastructure.Authorization.Validators.Abstract;

//namespace SchoolManagement.Infrastructure.Authorization.Validators.Concrete
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