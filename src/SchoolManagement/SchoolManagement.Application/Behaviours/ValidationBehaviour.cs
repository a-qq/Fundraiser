using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using SharedKernel.Infrastructure.Errors;
using SharedKernel.Infrastructure.Interfaces;

namespace SchoolManagement.Application.Behaviours
{
    internal sealed class
        ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, Result<TResponse, RequestError>>
        where TRequest : CommandRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<Result<TResponse, RequestError>> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<Result<TResponse, RequestError>> next)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                var validationResults =
                    await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
                var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToArray();

                if (failures.Length != 0)
                {
                    var errors = failures
                        .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                        .Select(em => new BodyFieldErrorModel(em.Key, em.ToArray()))
                        .ToArray();

                    return SharedRequestError.General.UnprocessableEntity(errors);
                }
            }

            return await next();
        }
    }
}