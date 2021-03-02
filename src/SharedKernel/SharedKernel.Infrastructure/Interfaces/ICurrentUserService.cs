using System.Security.Claims;

namespace SharedKernel.Infrastructure.Interfaces
{
    public interface ICurrentUserService
    {
        string UserId { get; }

        ClaimsPrincipal User { get; }
    }
}