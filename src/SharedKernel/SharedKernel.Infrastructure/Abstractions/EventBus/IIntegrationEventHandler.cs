using CSharpFunctionalExtensions;
using SharedKernel.Infrastructure.Concretes.Models;
using System.Threading.Tasks;

namespace SharedKernel.Infrastructure.Abstractions.EventBus
{
    public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler
        where TIntegrationEvent : IntegrationEvent
    {
        Task<Result> Handle(TIntegrationEvent @event);
    }

    public interface IIntegrationEventHandler
    {
    }
}
