using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.Extensions.Logging;
using SchoolManagement.Application.Common.Interfaces;
using SharedKernel.Infrastructure.Extensions;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Behaviors
{

    internal sealed class InternalQueryLoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, Maybe<TResponse>>
        where TRequest : IInternalQuery<TResponse>
    {
        private readonly ILogger<InternalQueryLoggingBehaviour<TRequest, TResponse>> _logger;
        public InternalQueryLoggingBehaviour(ILogger<InternalQueryLoggingBehaviour<TRequest, TResponse>> logger)
            => _logger = Guard.Against.Null(logger, nameof(logger));

        public async Task<Maybe<TResponse>> Handle(
            TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<Maybe<TResponse>> next)
        {
            _logger.LogInformation("----- Handling internal query {QueryName} ({@Query})", request.GetGenericTypeName(), request);

            var response = await next();

            if (response.HasValue)
                _logger.LogInformation("----- Internal query {QueryName} handled successfully - response: {@Response}", request.GetGenericTypeName(), response.Value);
            else
                _logger.LogInformation("----- Not found any matching results for internal query {QueryName}!", request.GetGenericTypeName());
            
            return response;
        }
    }
}