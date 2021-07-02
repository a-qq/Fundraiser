using IdentityModel;
using Microsoft.AspNetCore.Http;
using SharedKernel.Infrastructure.Abstractions.Common;
using System;
using System.Security.Claims;

namespace SharedKernel.Infrastructure.Concretes.Services
{
    public sealed class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ClaimsPrincipal User  => _httpContextAccessor.HttpContext?.User;

        public bool IsInRole(string role) => _httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;

        public bool HasGuidSubject => Guid.TryParse(this.User.FindFirstValue(JwtClaimTypes.Subject), out Guid _);
    }
}