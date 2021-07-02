//using MediatR;
//using SchoolManagement.Domain.SchoolAggregate.Members;
//using SchoolManagement.Infrastructure.Authorization.Validators.Abstract;

//namespace SchoolManagement.Infrastructure.Authorization.Validators.Concrete
//{
//    internal sealed class TeacherClaimPrincipalValidator : ClaimPrincipalValidatorForSchoolRolesBase,
//        IClaimsPrincipalValidator
//    {
//        public TeacherClaimPrincipalValidator(ISender mediator)
//            : base(mediator)
//        {
//        }

//        public override string RoleRequirement { get; } = SchoolRole.Teacher.ToString();
//    }
//}