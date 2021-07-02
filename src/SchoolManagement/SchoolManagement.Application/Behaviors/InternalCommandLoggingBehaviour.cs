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
    internal sealed class InternalCommandLoggingBehaviour<TRequest> : IPipelineBehavior<TRequest, Result>
        where TRequest : IInternalCommand
    {
        private readonly ILogger<InternalCommandLoggingBehaviour<TRequest>> _logger;
        public InternalCommandLoggingBehaviour(ILogger<InternalCommandLoggingBehaviour<TRequest>> logger)
            => _logger = Guard.Against.Null(logger, nameof(logger));

        public async Task<Result> Handle(
            TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<Result> next)
        {
            _logger.LogInformation("----- Handling internal command {CommandName} ({@Command})", request.GetGenericTypeName(), request);

            var response = await next();

            if (response.IsSuccess)
                _logger.LogInformation("----- Internal command {CommandName} handled successfully!", request.GetGenericTypeName());
            else
                _logger.LogInformation("----- Internal command {CommandName} failed - error(s): {Error}!", request.GetGenericTypeName(), response.Error);
            
            return response;
        }
    }
}