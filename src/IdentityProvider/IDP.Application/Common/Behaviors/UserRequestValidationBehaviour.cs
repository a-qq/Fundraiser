using CSharpFunctionalExtensions;
using FluentValidation;
using IDP.Application.Common.Abstractions;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IDP.Application.Common.Behaviors
{
    internal sealed class UserRequestValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, Result<TResponse>>
        where TRequest : IUserRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public UserRequestValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<Result<TResponse>> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<Result<TResponse>> next)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                var validationResults = await Task.WhenAll(
                    _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

                var failures = validationResults
                    .SelectMany(r => r.Errors).Where(f => f != null).ToList();

                if (failures.Count != 0)
                {
                    var errors = failures
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return Result.Failure<TResponse>(string.Join("\n", errors));
                }
            }

            return await next();
        }
    }
}