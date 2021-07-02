using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Models;
using FundraiserManagement.Application.Members.Commands.ArchiveMembers;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using SharedKernel.Infrastructure.Abstractions.EventBus;
using SharedKernel.Infrastructure.Concretes.Models;
using static FundraiserManagement.Application.MediatorModule;

namespace FundraiserManagement.Application.IntegrationEvents.Incoming
{
    internal sealed class MembersArchivedIntegrationEvent : IntegrationEvent
    {
        public IEnumerable<MemberArchivisationData> MembersData { get; }

        public MembersArchivedIntegrationEvent(IEnumerable<MemberArchivisationData> membersData)
        {
            MembersData = Guard.Against.NullOrEmpty(membersData, nameof(membersData));
        }
    }

    internal sealed class MembersArchivedIntegrationEventHandler : IIntegrationEventHandler<MembersArchivedIntegrationEvent>
    {
        private readonly ISender _mediator;
        private readonly ILogger<MembersArchivedIntegrationEventHandler> _logger;

        public MembersArchivedIntegrationEventHandler(
            ISender mediator,
            ILogger<MembersArchivedIntegrationEventHandler> logger)
        {
            _mediator = Guard.Against.Null(mediator, nameof(mediator));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        public async Task<Result> Handle(MembersArchivedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{AppName}"))
            {
                _logger.LogInformation(
                    "----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                    @event.Id, AppName, @event);


                var command = new ArchiveMembersCommand(@event.MembersData.Select(x => x.MemberId).ToList());

                var result = await _mediator.Send(
                    new IdentifiedCommand<ArchiveMembersCommand>(command, @event.Id));

                return result;
            }
        }
    }
}