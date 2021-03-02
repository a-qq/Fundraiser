using System.Security.Claims;
using System.Threading.Tasks;

namespace Backend.API.Authorization.Validators.Absrtact
{
    public interface IClaimsPrincipalValidator
    {
        Task<bool> IsValidAsync(ClaimsPrincipal principal);

        public string RoleRequirement { get; }
    }
}
