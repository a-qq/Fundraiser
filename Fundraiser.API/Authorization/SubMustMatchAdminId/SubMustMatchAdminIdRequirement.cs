using Microsoft.AspNetCore.Authorization;

namespace Fundraiser.API.Authorization.SubMustMatchAdminId
{
    public sealed class SubMustMatchAdminIdRequirement : IAuthorizationRequirement
    {
        public SubMustMatchAdminIdRequirement() { }
    }
}
