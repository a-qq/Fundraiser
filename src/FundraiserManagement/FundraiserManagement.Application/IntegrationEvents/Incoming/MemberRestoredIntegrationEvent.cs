using System.Threading.Tasks;
using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Models;
using FundraiserManagement.Application.Members.Commands.RestoreMember;
using FundraiserManagement.Domain.MemberAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using SharedKernel.Infrastructure.Abstractions.EventBus;
using SharedKernel.Infrastructure.Concretes.Models;
using static FundraiserManagement.Application.MediatorModule;

namespace FundraiserManagement.Application.IntegrationEvents.Incoming
{
    internal sealed class MemberRestoredIntegrationEvent : IntegrationEvent
    {
        public MemberRestoredIntegrationEvent(MemberId memberId)
        {
            MemberId = memberId;
        }

        public MemberId MemberId { get; }
    }

    internal sealed class MemberRestoredIntegrationEventHandler : IIntegrationEventHandler<MemberRestoredIntegrationEvent>
    {
        private readonly ISender _mediator;
        private readonly ILogger<MemberRestoredIntegrationEventHandler> _logger;

        public MemberRestoredIntegrationEventHandler(
            ISender mediator,
            ILogger<MemberRestoredIntegrationEventHandler> logger)
        {
            _mediator = Guard.Against.Null(mediator, nameof(mediator));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        public async Task<Result> Handle(MemberRestoredIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{AppName}"))
            {
                _logger.LogInformation(
                    "----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                    @event.Id, AppName, @event);

                var command = new RestoreMemberCommand(@event.MemberId);

                var result = await _mediator.Send(
                    new IdentifiedCommand<RestoreMemberCommand>(command, @event.Id));

                return result;
            }
        }
    }
}