using CSharpFunctionalExtensions;
using IdentityModel;
using MediatR;
using SchoolManagement.Application.Schools.Queries.GetAuthorizationData;
using SchoolManagement.Application.Schools.Queries.GetMember;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Domain.Utils;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Backend.API.Authorization.Validators.Absrtact
{
    internal abstract class ClaimPrincipalValidatorForSchoolRolesBase : ClaimPrincipalValidatorBase, IClaimsPrincipalValidator
    {
        private readonly ISender _mediator;

        public abstract string RoleRequirement { get; }
        protected Maybe<AuthorizationDto> AuthorizationData { get; private set; } = Maybe<AuthorizationDto>.None;
        public ClaimPrincipalValidatorForSchoolRolesBase(ISender mediator)
        {
            _mediator = mediator;
        }

        public async virtual Task<bool> IsValidAsync(ClaimsPrincipal principal)
        {
            if (!await HasValidData(principal))
                return false;

            return true;
        }

        private bool HasNotEmptySchoolId(ClaimsPrincipal principal, out Guid schoolId)
           => Guid.TryParse(principal.FindFirstValue(CustomClaimTypes.SchoolId), out schoolId) && schoolId != Guid.Empty;

        //private bool HasDefinedGender(ClaimsPrincipal principal, out GenderEnum gender)
        //{
        //    var result = Gender.ValidateAndConvert(principal.FindFirstValue(JwtClaimTypes.Gender));
        //    gender = result.IsSuccess ? result.Value : GenderEnum.Male;
        //    return result.IsSuccess;
        //}

        private bool HasInRangeGroupId(ClaimsPrincipal principal, out long? groupId)
        {
            groupId = null;

            var grouptokenValue = principal.FindFirstValue(CustomClaimTypes.GroupId);

            if (grouptokenValue is null)
                return true;

            if (long.TryParse(grouptokenValue, out long convertedGroupId) && convertedGroupId > 0)
            {
                groupId = convertedGroupId;
                return true;
            }

            return false;
        }

        private async Task<bool> HasValidData(ClaimsPrincipal principal)
        {
            if (!principal.IsInRole(RoleRequirement))
                return false;

            if (!HasValidUserId(principal, out Guid userId))
                return false;

            if (!HasNotEmptySchoolId(principal, out Guid schoolId))
                return false;

            //if (!HasDefinedGender(principal, out GenderEnum gender))
            //    return false;

            if (!HasInRangeGroupId(principal, out long? groupId))
                return false;

            var authorizationData = await _mediator.Send(new GetAuthorizationData(schoolId, userId));

            if (authorizationData.IsFailure)
                return false;

            AuthorizationData = authorizationData.Value;

            var schoolRoleOrError = Role.ValidateAndConvert(RoleRequirement);
            if (schoolRoleOrError.IsSuccess && authorizationData.Value.Role != schoolRoleOrError.Value)
                return false;

            //if (authorizationData.Value.Gender.ToString() != gender.ToString())
            //    return false;

            if (authorizationData.Value.GroupId != groupId)
                return false;

            return true;
        }
    }
}
