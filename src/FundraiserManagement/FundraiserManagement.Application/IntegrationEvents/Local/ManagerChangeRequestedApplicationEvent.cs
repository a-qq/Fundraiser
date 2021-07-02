using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Models;
using FundraiserManagement.Application.Fundraisers.Commands.ChangeManager;
using FundraiserManagement.Domain.Common.Models;
using FundraiserManagement.Domain.FundraiserAggregate.Fundraisers;
using FundraiserManagement.Domain.MemberAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using SharedKernel.Infrastructure.Abstractions.EventBus;
using SharedKernel.Infrastructure.Concretes.Models;
using System.Threading.Tasks;
using static FundraiserManagement.Application.MediatorModule;

namespace FundraiserManagement.Application.IntegrationEvents.Local
{
    internal sealed class ManagerChangeRequestedApplicationEvent : IntegrationEvent
    {
        public FundraiserId FundraiserId { get; }
        public MemberId ManagerId { get; }
        public SchoolId SchoolId { get; }

        public ManagerChangeRequestedApplicationEvent(
            FundraiserId fundraiserId, MemberId managerId, SchoolId schoolId)
        {
            FundraiserId = fundraiserId;
            ManagerId = managerId;
            SchoolId = schoolId;
        }
    }

    internal sealed class ManagerChangeRequestedApplicationEventHandler : IIntegrationEventHandler<ManagerChangeRequestedApplicationEvent>
    {
        private readonly ISender _mediator;
        private readonly ILogger<ManagerChangeRequestedApplicationEvent> _logger;

        public ManagerChangeRequestedApplicationEventHandler(
            ISender mediator,
            ILogger<ManagerChangeRequestedApplicationEvent> logger)
        {
            _mediator = Guard.Against.Null(mediator, nameof(mediator));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        public async Task<Result> Handle(ManagerChangeRequestedApplicationEvent @event)
        {
            using (LogContext.PushProperty("ApplicationEventContext", $"{@event.Id}-{AppName}"))
            {
                _logger.LogInformation(
                    "----- Handling application event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                    @event.Id, AppName, @event);

                var command = new ChangeManagerCommand(@event.SchoolId, @event.FundraiserId, @event.ManagerId, @event.Id.ToString());

                var result = await _mediator.Send(
                    new IdentifiedCommand<ChangeManagerCommand>(command, @event.Id));

                return result;
            }
        }
    }
}