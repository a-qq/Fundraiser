using Fundraiser.SharedKernel.Utils;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IDP.Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IDP.Infrastructure.Services
{
    public sealed class ProfileService : IProfileService
    {
        private readonly IMemoryCache _cache;
        private readonly IUserRepository _userRepository;

        public ProfileService(
            IMemoryCache memoryCache,
            IUserRepository userRepository)
        {
            _cache = memoryCache;
            _userRepository = userRepository;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subjectId = context.Subject.GetSubjectId();
            var userOrNull = await _userRepository.GetUserBySubjectAsync(subjectId);

            if (userOrNull.HasNoValue)
                return;

            var claimsForUser = userOrNull.Value.Claims
                .Select(c => new Claim(c.Type, c.Value)).ToList();

            context.AddRequestedClaims(claimsForUser);
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var subjectId = context.Subject.GetSubjectId();
            var userOrNull = await _userRepository.GetUserBySubjectAsync(subjectId);

            if (userOrNull.HasNoValue)
                return;

            if (userOrNull.Value.IsActive && !_cache.TryGetValue(SchemaNames.Authentiaction + userOrNull.Value.Subject, out _))
                _cache.Set(SchemaNames.Authentiaction + userOrNull.Value.Subject, userOrNull.Value, new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(new TimeSpan(0, 0, 3))
                    .SetSlidingExpiration(new TimeSpan(0, 0, 2)));

            context.IsActive = userOrNull.Value.IsActive;
        }
    }
}
