using Microsoft.AspNetCore.Authorization;
using SchoolManagement.Core.Interfaces;
using SchoolManagement.Core.SchoolAggregate.Members;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Fundraiser.API.Authorization.UserMustBeSchoolMember
{
    public sealed class UserMustBeSchoolMemberHandler : AuthorizationHandler<UserMustBeSchoolMemberRequirement>
    {
        private readonly ISchoolRepository _schoolRepository;

        public UserMustBeSchoolMemberHandler(ISchoolRepository schoolRepository)
        {
            _schoolRepository = schoolRepository;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserMustBeSchoolMemberRequirement requirement)
        {
            string subAsString = context.User.FindFirstValue("sub");

            if (!Guid.TryParse(subAsString, out Guid userId) || userId == Guid.Empty)
            {
                context.Fail();
                return;
            }

            string schoolIdAsString = context.User.FindFirstValue("school_id");

            if (!Guid.TryParse(schoolIdAsString, out Guid schoolId) || schoolId == Guid.Empty)
            {
                context.Fail();
                return;
            }

            var roles = context.User.FindAll("role");
            Claim schoolRole = roles.FirstOrDefault(r => Role.ValidateAndConvert(r.Value).IsSuccess);

            if (schoolRole == null)
            {
                context.Fail();
                return;
            }

            Role userRole = Role.Create(schoolRole.Value).Value;

            var currentUser = await _schoolRepository.GetSchoolMemberByIdAsync(schoolId, userId);

            if (currentUser.HasNoValue || !currentUser.Value.IsActive || currentUser.Value.Role != userRole)
            {
                context.Fail();
                return;
            }

            context.Succeed(requirement);

            return;
        }
    }
}
