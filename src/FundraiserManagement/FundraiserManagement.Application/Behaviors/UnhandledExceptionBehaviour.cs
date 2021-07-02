using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Infrastructure.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FundraiserManagement.Application.Behaviors
{
    internal sealed class UnhandledExceptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<UnhandledExceptionBehaviour<TRequest, TResponse>> _logger;

        public UnhandledExceptionBehaviour(ILogger<UnhandledExceptionBehaviour<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            try
            {
                return await next();
            }
            catch (Exception ex)
            {
                var requestName = request.GetGenericTypeName();

                _logger.LogError(ex, "----- Request {RequestName} failed with exception!", requestName);
    
                ex.Source = requestName;

                throw;
            }
        }
    }
}