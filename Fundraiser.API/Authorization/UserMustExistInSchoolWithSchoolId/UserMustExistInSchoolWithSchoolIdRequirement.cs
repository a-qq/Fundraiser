using Microsoft.AspNetCore.Authorization;

namespace Fundraiser.API.Authorization.UserMustExistInSchoolWithSchoolId
{
    public sealed class UserMustExistInSchoolWithSchoolIdRequirement : IAuthorizationRequirement
    {
        public UserMustExistInSchoolWithSchoolIdRequirement() { }
    }
}
