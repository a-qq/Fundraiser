using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using SharedKernel.Infrastructure.Interfaces;

namespace SharedKernel.Infrastructure.Implementations
{
    internal sealed class IdentityService : IIdentityService
    {
        private readonly IAuthorizationService _authorizationService;

        public IdentityService(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }


        public async Task<bool> AuthorizeAsync(ClaimsPrincipal user, string policyName)
        {
            if (user == null)
                return false;

            //throws if no policy found
            var result = await _authorizationService.AuthorizeAsync(user, policyName);

            return result.Succeeded;
        }
    }
}