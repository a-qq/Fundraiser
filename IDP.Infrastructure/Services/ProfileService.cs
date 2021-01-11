using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IDP.Core.Interfaces;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IDP.Infrastructure.Services
{
    public sealed class ProfileService : IProfileService
    {
        private readonly IUserRepository _userRepository;

        public ProfileService(IUserRepository userRepository)
        {
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
            context.IsActive = userOrNull.Value.IsActive;
        }
    }
}
