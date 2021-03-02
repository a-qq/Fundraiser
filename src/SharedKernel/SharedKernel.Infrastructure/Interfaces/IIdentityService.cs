using System.Security.Claims;
using System.Threading.Tasks;

namespace SharedKernel.Infrastructure.Interfaces
{
    public interface IIdentityService
    {
        Task<bool> AuthorizeAsync(ClaimsPrincipal user, string policyName);
    }
}