//using System;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using CSharpFunctionalExtensions;
//using MediatR;
//using SharedKernel.Domain.Utils;

//namespace SharedKernel.Infrastructure.Authorization.Validators.Abstract
//{
//    internal abstract class ClaimPrincipalValidatorForSchoolRolesBase : ClaimPrincipalValidatorBase,
//        IClaimsPrincipalValidator
//    {
//        private readonly ISender _mediator;

//        protected ClaimPrincipalValidatorForSchoolRolesBase(ISender mediator)
//        {
//            _mediator = mediator;
//        }

//        protected Maybe<AuthorizationDto> AuthorizationData { get; private set; } = Maybe<AuthorizationDto>.None;

//        public abstract string RoleRequirement { get; }

//        public virtual async Task<bool> IsValidAsync(ClaimsPrincipal principal)
//        {
//            if (!await HasValidData(principal))
//                return false;

//            return true;
//        }

//        private bool HasNotEmptySchoolId(ClaimsPrincipal principal, out Guid schoolId)
//        {
//            return Guid.TryParse(principal.FindFirstValue(CustomClaimTypes.SchoolId), out schoolId) &&
//                   schoolId != Guid.Empty;
//        }

//        //private bool HasDefinedGender(ClaimsPrincipal principal, out GenderEnum gender)
//        //{
//        //    var result = Gender.ValidateAndConvert(principal.FindFirstValue(JwtClaimTypes.Gender));
//        //    gender = result.IsSuccess ? result.Value : GenderEnum.Male;
//        //    return result.IsSuccess;
//        //}

//        private bool HasInRangeGroupId(ClaimsPrincipal principal, out long? groupId)
//        {
//            groupId = null;

//            var groupTokenValue = principal.FindFirstValue(CustomClaimTypes.GroupId);

//            if (groupTokenValue is null)
//                return true;

//            if (long.TryParse(groupTokenValue, out var convertedGroupId) && convertedGroupId > 0)
//            {
//                groupId = convertedGroupId;
//                return true;
//            }

//            return false;
//        }

//        private async Task<bool> HasValidData(ClaimsPrincipal principal)
//        {
//            if (!principal.IsInRole(RoleRequirement))
//                return false;

//            if (!HasValidUserId(principal, out var userId))
//                return false;

//            if (!HasNotEmptySchoolId(principal, out var schoolId))
//                return false;

//            //if (!HasDefinedGender(principal, out GenderEnum gender))
//            //    return false;

//            if (!HasInRangeGroupId(principal, out var groupId))
//                return false;

//            var authorizationData = await _mediator.Send(new GetAuthorizationData(schoolId, userId));

//            if (authorizationData.HasNoValue)
//                return false;

//            AuthorizationData = authorizationData.Value;

//            var schoolRoleOrError = Role.ValidateAndConvert(RoleRequirement);
//            if (schoolRoleOrError.IsSuccess && authorizationData.Value.Role != schoolRoleOrError.Value)
//                return false;

//            //if (authorizationData.Value.Gender.ToString() != gender.ToString())
//            //    return false;

//            if (authorizationData.Value.GroupId != groupId)
//                return false;

//            return true;
//        }
//    }
//}