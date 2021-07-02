using System.Threading.Tasks;
using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Models;
using FundraiserManagement.Application.Members.Commands.DivestTreasurer;
using FundraiserManagement.Domain.MemberAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using SharedKernel.Domain.Constants;
using SharedKernel.Infrastructure.Abstractions.EventBus;
using SharedKernel.Infrastructure.Concretes.Models;
using static FundraiserManagement.Application.MediatorModule;

namespace FundraiserManagement.Application.IntegrationEvents.Incoming
{
    internal sealed class TreasurerDivestedIntegrationEvent : IntegrationEvent
    {
        public TreasurerDivestedIntegrationEvent(MemberId treasurerId, bool isActive)
        {
            TreasurerId = treasurerId;
            IsActive = isActive;
            RemovedRole = GroupRoles.Treasurer;
        }

        public MemberId TreasurerId { get; }
        public string RemovedRole { get; }
        public bool IsActive { get; }
    }

    internal sealed class TreasurerDivestedIntegrationEventHandler : IIntegrationEventHandler<TreasurerDivestedIntegrationEvent>
    {

        private readonly ISender _mediator;
        private readonly ILogger<TreasurerDivestedIntegrationEventHandler> _logger;

        public TreasurerDivestedIntegrationEventHandler(
            ISender mediator,
            ILogger<TreasurerDivestedIntegrationEventHandler> logger)
        {
            _mediator = Guard.Against.Null(mediator, nameof(mediator));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        public async Task<Result> Handle(TreasurerDivestedIntegrationEvent @event)
        {
            if(!@event.IsActive)
                return Result.Success();

            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{AppName}"))
            {
                _logger.LogInformation(
                    "----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                    @event.Id, AppName, @event);

                var command = new DivestTreasurerCommand(@event.TreasurerId);

                var result = await _mediator.Send(
                    new IdentifiedCommand<DivestTreasurerCommand>(command, @event.Id));

                return result;
            }
        }
    }
}