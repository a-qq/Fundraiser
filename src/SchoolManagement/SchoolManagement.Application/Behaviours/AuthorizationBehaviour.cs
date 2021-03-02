using CSharpFunctionalExtensions;
using MediatR;
using SchoolManagement.Application.Common.Security;
using SharedKernel.Infrastructure.Errors;
using SharedKernel.Infrastructure.Interfaces;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Behaviours
{
    internal sealed class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, Result<TResponse, RequestError>>
        where TRequest : CommandRequest<TResponse>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IIdentityService _identityService;

        public AuthorizationBehaviour(
            ICurrentUserService currentUserService,
            IIdentityService identityService)
        {
            _currentUserService = currentUserService;
            _identityService = identityService;
        }

        public async Task<Result<TResponse, RequestError>> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<Result<TResponse, RequestError>> next)
        {
            var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>();
            
            if (authorizeAttributes.Any())
            {
                // Must be authenticated user
                if (_currentUserService.UserId == null)
                {
                    return SharedRequestError.General.UnauthorizedAccess();
                }

                bool authorized = false;

                // Policy-based authorization
                var authorizeAttributesWithPolicies = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Policy));
                if (authorizeAttributesWithPolicies.Any())
                {
                    //admin is authorized to perform any command with required authorization
                    if (authorized)
                        return await next();

                    foreach (var policy in authorizeAttributesWithPolicies.Select(a => a.Policy))
                    {
                        authorized = await _identityService.AuthorizeAsync(_currentUserService.User, policy);

                        if (!authorized)
                            return SharedRequestError.General.ForbiddenAccess();
                    }
                }
            }

            // User is authorized / authorization not required
            return await next();
        }
    }
}
