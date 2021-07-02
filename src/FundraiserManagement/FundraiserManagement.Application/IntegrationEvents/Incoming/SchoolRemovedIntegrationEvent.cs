using System.Threading.Tasks;
using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Models;
using FundraiserManagement.Application.Members.Commands.DeleteSchoolMembers;
using FundraiserManagement.Domain.Common.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using SharedKernel.Infrastructure.Abstractions.EventBus;
using SharedKernel.Infrastructure.Concretes.Models;
using static FundraiserManagement.Application.MediatorModule;

namespace FundraiserManagement.Application.IntegrationEvents.Incoming
{
    internal sealed class SchoolRemovedIntegrationEvent : IntegrationEvent
    {
        public SchoolRemovedIntegrationEvent(SchoolId schoolId)
        {
            SchoolId = schoolId;
        }

        public SchoolId SchoolId { get; }
    }

    internal sealed class SchoolRemovedIntegrationEventHandler : IIntegrationEventHandler<SchoolRemovedIntegrationEvent>
    {
        private readonly ISender _mediator;
        private readonly ILogger<SchoolRemovedIntegrationEventHandler> _logger;

        public SchoolRemovedIntegrationEventHandler(
            ISender mediator,
            ILogger<SchoolRemovedIntegrationEventHandler> logger)
        {
            _mediator = Guard.Against.Null(mediator, nameof(mediator));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }
        public async Task<Result> Handle(SchoolRemovedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{AppName}"))
            {
                _logger.LogInformation(
                    "----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                    @event.Id, AppName, @event);

                var command = new DeleteSchoolMembersCommand(@event.SchoolId);

                var result = await _mediator.Send(
                    new IdentifiedCommand<DeleteSchoolMembersCommand>(command, @event.Id));

                return result;
            }
        }
    }
}