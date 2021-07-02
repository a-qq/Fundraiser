using System;
using System.Threading.Tasks;
using SharedKernel.Infrastructure.Concretes.Models;

namespace SharedKernel.Infrastructure.Abstractions.Common
{
    public interface IIntegrationEventService
    {
        Task PublishEventsThroughEventBusAsync(Guid transactionId);
        Task AddAndSaveEventAsync(IntegrationEvent evt);
    }
}