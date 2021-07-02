using Autofac;
using MediatR;
using System.Threading.Tasks;
using FundraiserManagement.Application.Common.Interfaces;
using FundraiserManagement.Application.Common.Interfaces.Mediator;

namespace FundraiserManagement.IntegrationTests
{
    public static class QueriesExecutor
    {
        public static async Task<TResult> Execute<TResult>(IQuery<TResult> query)
        {
            using (var scope = CompositionRoot.BeginLifetimeScope())
            {
                var mediator = scope.Resolve<IMediator>();

                return await mediator.Send(query);
            }
        }
    }
}
