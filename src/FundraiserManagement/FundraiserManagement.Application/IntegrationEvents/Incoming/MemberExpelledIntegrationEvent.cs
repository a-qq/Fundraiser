using System.Threading.Tasks;
using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Models;
using FundraiserManagement.Application.Members.Commands.DeleteMember;
using FundraiserManagement.Domain.MemberAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using SharedKernel.Infrastructure.Abstractions.EventBus;
using SharedKernel.Infrastructure.Concretes.Models;
using static FundraiserManagement.Application.MediatorModule;

namespace FundraiserManagement.Application.IntegrationEvents.Incoming
{
    internal sealed class MemberExpelledIntegrationEvent : IntegrationEvent
    {
        public MemberExpelledIntegrationEvent(MemberId memberId, bool isActive)
        {
            MemberId = memberId;
            IsActive = isActive;
        }

        public MemberId MemberId { get; }
        public bool IsActive { get; }
    }

    internal sealed class MemberExpelledIntegrationEventHandler : IIntegrationEventHandler<MemberExpelledIntegrationEvent>
    {
        private readonly ISender _mediator;
        private readonly ILogger<MemberExpelledIntegrationEventHandler> _logger;

        public MemberExpelledIntegrationEventHandler(
            ISender mediator,
            ILogger<MemberExpelledIntegrationEventHandler> logger)
        {
            _mediator = Guard.Against.Null(mediator, nameof(mediator));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        public async Task<Result> Handle(MemberExpelledIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{AppName}"))
            {
                _logger.LogInformation(
                    "----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                    @event.Id, AppName, @event);

                var command = new DeleteMemberCommand(@event.MemberId);

                var result = await _mediator.Send(
                    new IdentifiedCommand<DeleteMemberCommand>(command, @event.Id));

                return result;
            }
        }
    }
}