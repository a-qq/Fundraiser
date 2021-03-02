using System.Security.Claims;
using System.Threading.Tasks;

namespace Backend.API.Authorization.Validators.Absrtact
{
    public interface IClaimsPrincipalValidator
    {
        public string RoleRequirement { get; }
        Task<bool> IsValidAsync(ClaimsPrincipal principal);
    }
}