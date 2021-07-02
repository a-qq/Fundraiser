using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using SharedKernel.Infrastructure.Abstractions.Common;

namespace SharedKernel.Infrastructure.Concretes.Services
{
    public sealed class IdentityService : IIdentityService
    {
        private readonly IAuthorizationService _authorizationService;

        public IdentityService(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }


        public async Task<bool> AuthorizeAsync(ClaimsPrincipal user, string policyName, object request = null)
        {
            if (user == null)
                return false;

            //throws if no policy found
            var result = await _authorizationService.AuthorizeAsync(user, request, policyName);

            return result.Succeeded;
        }

        public async Task<bool> AuthorizeAsync(ClaimsPrincipal user, IAuthorizationRequirement requirement, object request = null)
        {
            if (user == null)
                return false;

            //throws if no policy found
            var result = await _authorizationService.AuthorizeAsync(user, request, requirement);

            return result.Succeeded;
        }
    }
}