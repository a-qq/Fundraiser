using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Interfaces;
using FundraiserManagement.Application.Common.Security;
using MediatR;
using SharedKernel.Infrastructure.Abstractions.Common;
using SharedKernel.Infrastructure.Errors;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FundraiserManagement.Application.Common.Interfaces.Mediator;

namespace FundraiserManagement.Application.Behaviors
{
    internal sealed class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, Result<TResponse, RequestError>>
        where TRequest : IUserRequest<TResponse>
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

        public async Task<Result<TResponse, RequestError>> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<Result<TResponse, RequestError>> next)
        {
            var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>().ToList();

            if (authorizeAttributes.Any())
            {
                // Must be authenticated user
                if (!_currentUserService.HasGuidSubject)
                    return Result.Failure<TResponse, RequestError>(SharedRequestError.General.UnauthorizedAccess());

                // Policy-based authorization
                var authorizeAttributesWithPolicies =
                    authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Policy)).ToList();

                if (authorizeAttributesWithPolicies.Any())
                {
                    foreach (var policy in authorizeAttributesWithPolicies.Select(a => a.Policy))
                    {
                        var authorized = await _identityService.AuthorizeAsync(_currentUserService.User, policy);

                        if (!authorized)
                            return Result.Failure<TResponse, RequestError>(SharedRequestError.General.ForbiddenAccess());
                    }
                }
            }

            // User is authorized / authorization not required
            return await next();
        }
    }
}