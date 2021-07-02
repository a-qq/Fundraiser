using MediatR;
using Microsoft.Extensions.Logging;
using SK = SharedKernel.Infrastructure.Concretes.Services;

namespace SchoolManagement.Infrastructure.Services
{
    public sealed class DomainEventService : SK.DomainEventService
    {
        public DomainEventService(
            ILogger<SK.DomainEventService> logger,
            IPublisher mediator,
            EventReducersManager eventReducer) 
            : base(logger, mediator, eventReducer)
        {
        }
    }
}