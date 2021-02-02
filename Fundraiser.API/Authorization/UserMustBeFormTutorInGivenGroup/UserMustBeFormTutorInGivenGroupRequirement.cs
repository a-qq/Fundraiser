using Microsoft.AspNetCore.Authorization;

namespace Fundraiser.API.Authorization.UserMustBeFormTutorInGivenGroup
{
    internal sealed class UserMustBeFormTutorInGivenGroupRequirement : IAuthorizationRequirement
    {
        public UserMustBeFormTutorInGivenGroupRequirement() { }
    }
}
