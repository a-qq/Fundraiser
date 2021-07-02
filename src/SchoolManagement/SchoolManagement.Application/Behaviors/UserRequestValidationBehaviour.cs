using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using SchoolManagement.Application.Common.Interfaces;
using SharedKernel.Infrastructure.Errors;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Behaviors
{
    internal sealed class UserRequestValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, Result<TResponse, RequestError>>
        where TRequest : IUserRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public UserRequestValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<Result<TResponse, RequestError>> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<Result<TResponse, RequestError>> next)
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
                        .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                        .Select(em => new BodyFieldErrorModel(em.Key, em.ToList()))
                        .ToList();

                    return Result.Failure<TResponse, RequestError>(SharedRequestError.General.UnprocessableEntity(errors));
                }
            }

            return await next();
        }
    }
}