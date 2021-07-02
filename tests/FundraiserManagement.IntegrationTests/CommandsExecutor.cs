using Autofac;
using MediatR;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Interfaces;
using FundraiserManagement.Application.Common.Interfaces.Mediator;

namespace FundraiserManagement.IntegrationTests
{
    public static class CommandsExecutor
    {
        public static async Task<TResult> Execute<TResult>(ICommand<TResult> command)
            where TResult : IResult
        {
            using (var scope = CompositionRoot.BeginLifetimeScope())
            {
                var mediator = scope.Resolve<IMediator>();
                return await mediator.Send(command);
            }
        }
    }
}
