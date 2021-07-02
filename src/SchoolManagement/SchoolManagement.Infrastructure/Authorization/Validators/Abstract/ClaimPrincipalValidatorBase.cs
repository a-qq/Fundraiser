//using System;
//using System.Security.Claims;
//using IdentityModel;

//namespace SchoolManagement.Infrastructure.Authorization.Validators.Abstract
//{
//    internal abstract class ClaimPrincipalValidatorBase
//    {
//        protected bool HasValidUserId(ClaimsPrincipal principal, out Guid userId)
//        {
//            return Guid.TryParse(principal.FindFirstValue(JwtClaimTypes.Subject), out userId) && userId != Guid.Empty;
//        }
//    }
//}