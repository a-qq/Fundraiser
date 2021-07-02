using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Models;
using FundraiserManagement.Application.Members.Commands.DivestTreasurers;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using SharedKernel.Domain.Constants;
using SharedKernel.Infrastructure.Abstractions.EventBus;
using SharedKernel.Infrastructure.Concretes.Models;
using static FundraiserManagement.Application.MediatorModule;

namespace FundraiserManagement.Application.IntegrationEvents.Incoming
{
    internal sealed class TreasurersDivestedIntegrationEvent : IntegrationEvent
    {
        public string RemovedRole { get; }
        public IEnumerable<MemberIsActiveModel> TreasurersData { get; }

        public TreasurersDivestedIntegrationEvent(IEnumerable<MemberIsActiveModel> treasurersData)
        {
            RemovedRole = GroupRoles.Treasurer;
            TreasurersData = treasurersData;
        }
    }

    internal sealed class TreasurersDivestedIntegrationEventHandler : IIntegrationEventHandler<TreasurersDivestedIntegrationEvent>
    {
        private readonly ISender _mediator;
        private readonly ILogger<TreasurersDivestedIntegrationEventHandler> _logger;

        public TreasurersDivestedIntegrationEventHandler(
            ISender mediator,
            ILogger<TreasurersDivestedIntegrationEventHandler> logger)
        {
            _mediator = Guard.Against.Null(mediator, nameof(mediator));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        public async Task<Result> Handle(TreasurersDivestedIntegrationEvent @event)
        {
            var treasurerIds = @event.TreasurersData.Where(d => d.IsActive).Select(d => d.MemberId).ToList();
            if (!treasurerIds.Any())
                return Result.Success();

            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{AppName}"))
            {
                _logger.LogInformation(
                    "----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                    @event.Id, AppName, @event);

                var command = new DivestTreasurersCommand(treasurerIds);

                var result = await _mediator.Send(
                    new IdentifiedCommand<DivestTreasurersCommand>(command, @event.Id));

                return result;
            }
        }
    }
}