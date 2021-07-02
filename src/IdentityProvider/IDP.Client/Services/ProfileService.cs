using Ardalis.GuardClauses;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IDP.Application.Users.Queries;
using IDP.Domain.UserAggregate.ValueObjects;
using MediatR;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IDP.Client.Services
{
    public sealed class ProfileService : IProfileService
    {
        private readonly ISender _mediator;

        public ProfileService(ISender mediator)
        {
            _mediator = Guard.Against.Null(mediator, nameof(mediator));
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subjectId = context.Subject.GetSubjectId();
            if (Subject.Validate(subjectId).IsFailure)
                return;

            var claimsOrNone = await _mediator.Send(new GetUserClaimsQuery(subjectId));

            if (claimsOrNone.HasNoValue || !claimsOrNone.Value.Any())
                return;

            var claimsForUser = claimsOrNone.Value.Select(c => new Claim(c.Type, c.Value)).ToList();

            context.AddRequestedClaims(claimsForUser);
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var subjectId = context.Subject.GetSubjectId();

            var isActive = await _mediator.Send(new IsUserActiveQuery(subjectId));

            if (isActive.HasNoValue)
                return;

            //if (userOrNone.Value.IsActive &&
            //    !_cache.TryGetValue(SchemaNames.Authentication + userOrNone.Value.Subject, out _))
            //    _cache.Set(SchemaNames.Authentication + userOrNone.Value.Subject, userOrNone.Value,
            //        new MemoryCacheEntryOptions()
            //            .SetAbsoluteExpiration(new TimeSpan(0, 0, 3))
            //            .SetSlidingExpiration(new TimeSpan(0, 0, 2)));

            context.IsActive = isActive.Value;
        }
    }
}