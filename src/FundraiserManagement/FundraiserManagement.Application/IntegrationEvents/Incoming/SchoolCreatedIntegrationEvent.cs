using System.Threading.Tasks;
using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Models;
using FundraiserManagement.Application.Schools.CreateSchoolWithPaymentsAccount;
using FundraiserManagement.Domain.Common.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using SharedKernel.Infrastructure.Abstractions.EventBus;
using SharedKernel.Infrastructure.Concretes.Models;
using static FundraiserManagement.Application.MediatorModule;

namespace FundraiserManagement.Application.IntegrationEvents.Incoming
{
    internal sealed class SchoolCreatedIntegrationEvent : IntegrationEvent
    {
        public SchoolId SchoolId { get; }

        public SchoolCreatedIntegrationEvent(SchoolId schoolId)
        {
            SchoolId = schoolId;
        }
    }

    internal sealed class SchoolCreatedIntegrationEventHandler : IIntegrationEventHandler<SchoolCreatedIntegrationEvent>
    {
        private readonly ISender _mediator;
        private readonly ILogger<SchoolCreatedIntegrationEventHandler> _logger;

        public SchoolCreatedIntegrationEventHandler(
            ISender mediator,
            ILogger<SchoolCreatedIntegrationEventHandler> logger)
        {
            _mediator = Guard.Against.Null(mediator, nameof(mediator));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        public async Task<Result> Handle(SchoolCreatedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{AppName}"))
            {
                _logger.LogInformation(
                    "----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                    @event.Id, AppName, @event);

                var command = new CreateSchoolWithPaymentsAccountCommand(@event.SchoolId);

                var result = await _mediator.Send(new IdentifiedCommand<CreateSchoolWithPaymentsAccountCommand>(command, @event.Id));

                return result;
            }
        }
    }
}