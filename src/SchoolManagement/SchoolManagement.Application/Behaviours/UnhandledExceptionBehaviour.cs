using MediatR;
using SchoolManagement.Application.Common.Security;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Behaviours
{
    internal sealed class UnhandledExceptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            try
            {
                return await next();
            }
            catch (Exception ex)
            {
                var requestName = typeof(TRequest).Name;
                ex.Source = requestName;
              
                throw;
            }
        }
    }
}
