using Ardalis.GuardClauses;
using FluentValidation;
using IDP.Application.Common.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SharedKernel.Infrastructure.Extensions;

namespace IDP.Application.Common.Behaviors
{
    internal sealed class InternalRequestValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IInternalRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        private readonly ILogger<InternalRequestValidationBehaviour<TRequest, TResponse>> _logger;

        public InternalRequestValidationBehaviour(
            IEnumerable<IValidator<TRequest>> validators,
            ILogger<InternalRequestValidationBehaviour<TRequest, TResponse>> logger)
        {
            _validators = validators;
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var typeName = request.GetGenericTypeName();

            _logger.LogInformation("----- Validating request {RequestType}", typeName);

            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
                var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

                if (failures.Count != 0)
                {
                    _logger.LogWarning("Validation errors - {RequestType} - Request: {@Request} - Errors: {@ValidationErrors}", typeName, request, failures);
                    throw new ValidationException(failures);
                }
            }
            return await next();
        }
    }
}