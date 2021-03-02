using Microsoft.AspNetCore.Http;
using SharedKernel.Infrastructure.Interfaces;
using System.Security.Claims;

namespace Backend.API.Services
{
    internal sealed class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User;

        public string UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue("sub");
    }
}
