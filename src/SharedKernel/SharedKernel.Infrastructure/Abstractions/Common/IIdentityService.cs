using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace SharedKernel.Infrastructure.Abstractions.Common
{
    public interface IIdentityService
    {
        Task<bool> AuthorizeAsync(ClaimsPrincipal user, string policyName, object request = null);
        Task<bool> AuthorizeAsync(ClaimsPrincipal user, IAuthorizationRequirement requirement, object request = null);
    }
}