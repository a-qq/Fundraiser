using System.Threading.Tasks;
using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Models;
using FundraiserManagement.Application.Members.Commands.ArchiveMember;
using FundraiserManagement.Domain.MemberAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using SharedKernel.Infrastructure.Abstractions.EventBus;
using SharedKernel.Infrastructure.Concretes.Models;
using static FundraiserManagement.Application.MediatorModule;

namespace FundraiserManagement.Application.IntegrationEvents.Incoming
{
    internal sealed class MemberArchivedIntegrationEvent : IntegrationEvent
    {
        public MemberArchivedIntegrationEvent(MemberId memberId, string groupRole)
        {
            MemberId = memberId;
            GroupRole = groupRole;
        }

        public MemberId MemberId { get; }
        public string GroupRole { get; }
    }

    internal sealed class MemberArchivedIntegrationEventHandler : IIntegrationEventHandler<MemberArchivedIntegrationEvent>
    {
        private readonly ISender _mediator;
        private readonly ILogger<MemberArchivedIntegrationEventHandler> _logger;

        public MemberArchivedIntegrationEventHandler(
            ISender mediator,
            ILogger<MemberArchivedIntegrationEventHandler> logger)
        {
            _mediator = Guard.Against.Null(mediator, nameof(mediator));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        public async Task<Result> Handle(MemberArchivedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{AppName}"))
            {
                _logger.LogInformation(
                    "----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                    @event.Id, AppName, @event);

                var command = new ArchiveMemberCommand(@event.MemberId);

                var result = await _mediator.Send(
                    new IdentifiedCommand<ArchiveMemberCommand>(command, @event.Id));

                return result;
            }
        }
    }
}