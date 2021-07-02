using SharedKernel.Domain.Utils;
using System;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using SharedKernel.Domain.Constants;

namespace SharedKernel.Infrastructure.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool IsInGroup(this ClaimsPrincipal cp, Guid groupId)
            => cp.HasClaim(CustomClaimTypes.GroupId, groupId.ToString());

        public static bool IsInSchool(this ClaimsPrincipal cp, Guid schoolId)
            => cp.HasClaim(CustomClaimTypes.SchoolId, schoolId.ToString());

        public static bool IsInGroupRole(this ClaimsPrincipal cp)
            => cp.FindAll(JwtClaimTypes.Role).Any(r => Enum.TryParse(typeof(GroupRole), r.Value, true, out _));

        public static bool IsInSchoolRole(this ClaimsPrincipal cp)
            => cp.FindAll(JwtClaimTypes.Role).Any(r => Enum.TryParse(typeof(SchoolRole), r.Value, true, out _));

        public static bool HasGender(this ClaimsPrincipal cp, Gender gender)
            => cp.FindAll(JwtClaimTypes.Gender).Any(r => Enum.TryParse(r.Value, true, out Gender foundGender) && gender == foundGender);

        public static Guid Subject(this ClaimsPrincipal cp)
            => Guid.Parse(cp.FindFirstValue(JwtClaimTypes.Subject));
    }
}