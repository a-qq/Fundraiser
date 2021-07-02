using System.Security.Claims;

namespace SharedKernel.Infrastructure.Abstractions.Common
{
    public interface ICurrentUserService
    {
        bool HasGuidSubject { get; }
        bool IsInRole(string role);
        ClaimsPrincipal User { get; }
    }
}