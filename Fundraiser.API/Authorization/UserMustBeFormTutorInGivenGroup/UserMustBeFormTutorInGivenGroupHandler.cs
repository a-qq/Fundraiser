using Fundraiser.SharedKernel.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using SchoolManagement.Core.Interfaces;
using SchoolManagement.Core.SchoolAggregate.Groups;
using SchoolManagement.Core.SchoolAggregate.Members;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Fundraiser.API.Authorization.UserMustBeFormTutorInGivenGroup
{
    internal sealed class UserMustBeFormTutorInGivenGroupHandler : AuthorizationHandler<UserMustBeFormTutorInGivenGroupRequirement>
    {
        private readonly ISchoolRepository _schoolRepository;
        private readonly IMemoryCache _cache;
        private readonly IHttpContextAccessor _accessor;

        public UserMustBeFormTutorInGivenGroupHandler(
            ISchoolRepository schoolRepository,
            IMemoryCache memoryCache,
            IHttpContextAccessor accessor)
        {
            this._schoolRepository = schoolRepository;
            this._cache = memoryCache;
            this._accessor = accessor;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserMustBeFormTutorInGivenGroupRequirement requirement)
        {
            string subAsString = context.User.FindFirstValue("sub");

            if (!Guid.TryParse(subAsString, out Guid userId) || userId == Guid.Empty)
            {
                context.Fail();
                return;
            }

            string schoolIdAsString = context.User.FindFirstValue("school_id");

            if (!Guid.TryParse(schoolIdAsString, out Guid schoolId) || schoolId == Guid.Empty)
            {
                context.Fail();
                return;
            }

            var roles = context.User.FindAll("role");
            Claim schoolRole = roles.FirstOrDefault(r => Role.ValidateAndConvert(r.Value).IsSuccess);

            if (schoolRole == null)
            {
                context.Fail();
                return;
            }

            Role userRole = Role.Create(schoolRole.Value).Value;

            var currentUser = await _schoolRepository.GetSchoolMemberByIdAsync(schoolId, userId);

            if (currentUser.HasNoValue || !currentUser.Value.IsActive
                 || currentUser.Value.Role != userRole || currentUser.Value.IsArchived)
            {
                context.Fail();
                return;
            }

            if (currentUser.Value.Role == Role.Teacher)
            {
                if (!roles.Any(c => c.Value == GroupRoles.FormTutor))
                {
                    context.Fail();
                    return;
                }

                if (!_accessor.HttpContext.Request.RouteValues.TryGetValue("groupId", out var groupIdAsObject))
                    throw new InvalidOperationException(nameof(UserMustBeFormTutorInGivenGroupHandler));

                //will fail on modelbinding or actionfilter returning 422
                if (!long.TryParse(groupIdAsObject as string, out long groupId) || groupId < 1)
                {
                    context.Succeed(requirement);
                    return;
                }

                var groupOrNone = await _schoolRepository.GetGroupWithFormTutorByIdAsync(schoolId, groupId);

                if (groupOrNone.HasNoValue || groupOrNone.Value?.FormTutor?.Id != userId)
                {
                    context.Fail();
                    return;
                }

                _cache.Set(SchemaNames.Management + nameof(Group) + groupId, groupOrNone.Value, new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(new TimeSpan(0, 0, 5))
                        .SetSlidingExpiration(new TimeSpan(0, 0, 3)));
            }

            _cache.Set(SchemaNames.Management + userId, currentUser.Value, new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(new TimeSpan(0, 0, 5))
                        .SetSlidingExpiration(new TimeSpan(0, 0, 3)));

            context.Succeed(requirement);
            return;

        }
    }
}
