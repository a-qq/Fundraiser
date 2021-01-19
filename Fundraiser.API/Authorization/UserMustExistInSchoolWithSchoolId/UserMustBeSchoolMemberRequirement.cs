using Microsoft.AspNetCore.Authorization;

namespace Fundraiser.API.Authorization.UserMustBeSchoolMember
{
    public sealed class UserMustBeSchoolMemberRequirement : IAuthorizationRequirement
    {
        public UserMustBeSchoolMemberRequirement() { }
    }
}
