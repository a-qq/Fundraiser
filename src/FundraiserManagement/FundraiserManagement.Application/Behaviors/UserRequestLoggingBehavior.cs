using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Interfaces;
using FundraiserManagement.Application.Common.Interfaces.Mediator;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Infrastructure.Errors;
using SharedKernel.Infrastructure.Extensions;

namespace FundraiserManagement.Application.Behaviors
{
    internal sealed class UserRequestLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, Result<TResponse, RequestError>>
        where TRequest : IUserRequest<TResponse>
    {
        private readonly ILogger<UserRequestLoggingBehavior<TRequest, TResponse>> _logger;
        public UserRequestLoggingBehavior(ILogger<UserRequestLoggingBehavior<TRequest, TResponse>> logger) 
            => _logger = Guard.Against.Null(logger, nameof(logger));

        public async Task<Result<TResponse, RequestError>> Handle(
            TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<Result<TResponse, RequestError>> next)
        {
            _logger.LogInformation("----- Handling user request {RequestName} ({@request})", request.GetGenericTypeName(), request);

            var response = await next();
            if (response.IsSuccess)
            {
                if (response.Value is Unit)
                    _logger.LogInformation("----- Request {RequestName} handled successfully!", request.GetGenericTypeName());
                else
                    _logger.LogInformation("----- Request {RequestName} handled successfully - response: {@Response}", request.GetGenericTypeName(), response.Value);
            }
            else
            {
                _logger.LogInformation("----- Request {RequestName} failed - error(s): {@Error}", request.GetGenericTypeName(), response.Error);
            }

            return response;
        }
    }
}
