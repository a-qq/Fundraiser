using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Models;
using FundraiserManagement.Application.Fundraisers.Commands.OpenFundraiser;
using FundraiserManagement.Domain.Common.Models;
using FundraiserManagement.Domain.FundraiserAggregate.Fundraisers;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using SharedKernel.Infrastructure.Abstractions.EventBus;
using SharedKernel.Infrastructure.Concretes.Models;
using System.Threading.Tasks;
using static FundraiserManagement.Application.MediatorModule;

namespace FundraiserManagement.Application.IntegrationEvents.Local
{
    internal sealed class FundraiserOpeningRequestedApplicationEvent : IntegrationEvent
    {
        public FundraiserId FundraiserId { get; }
        public SchoolId SchoolId { get; }

        public FundraiserOpeningRequestedApplicationEvent(FundraiserId fundraiserId, SchoolId schoolId)
        {
            FundraiserId = fundraiserId;
            SchoolId = schoolId;
        }
    }

    internal sealed class FundraiserOpeningRequestedApplicationEventHandler : IIntegrationEventHandler<FundraiserOpeningRequestedApplicationEvent>
    {
        private readonly ISender _mediator;
        private readonly ILogger<FundraiserOpeningRequestedApplicationEvent> _logger;

        public FundraiserOpeningRequestedApplicationEventHandler(
            ISender mediator,
            ILogger<FundraiserOpeningRequestedApplicationEvent> logger)
        {
            _mediator = Guard.Against.Null(mediator, nameof(mediator));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        public async Task<Result> Handle(FundraiserOpeningRequestedApplicationEvent @event)
        { 
            using (LogContext.PushProperty("ApplicationEventContext", $"{@event.Id}-{AppName}"))
            {
                _logger.LogInformation(
                    "----- Handling application event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                    @event.Id, AppName, @event);

                var command = new OpenFundraiserCommand(@event.FundraiserId, @event.SchoolId);

                var result = await _mediator.Send(
                    new IdentifiedCommand<OpenFundraiserCommand>(command, @event.Id));

                return result;
            }
        }
    }
}