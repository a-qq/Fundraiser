using System.Threading.Tasks;
using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Models;
using FundraiserManagement.Application.Members.Commands.PromoteTreasurer;
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
    internal sealed class TreasurerPromotedIntegrationEvent : IntegrationEvent
    {
        public TreasurerPromotedIntegrationEvent(MemberId studentId, bool isActive)
        {
            StudentId = studentId;
            AssignedRole = GroupRoles.Treasurer;
            IsActive = isActive;
        }

        public MemberId StudentId { get; }
        public string AssignedRole { get; }
        public bool IsActive { get; }
    }

    internal sealed class
        TreasurerPromotedIntegrationEventHandler : IIntegrationEventHandler<TreasurerPromotedIntegrationEvent>
    {
        private readonly ISender _mediator;
        private readonly ILogger<TreasurerPromotedIntegrationEventHandler> _logger;

        public TreasurerPromotedIntegrationEventHandler(
            ISender mediator,
            ILogger<TreasurerPromotedIntegrationEventHandler> logger)
        {
            _mediator = Guard.Against.Null(mediator, nameof(mediator));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        public async Task<Result> Handle(TreasurerPromotedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{AppName}"))
            {
                _logger.LogInformation(
                    "----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                    @event.Id, AppName, @event);

                var command = new PromoteTreasurerCommand(@event.StudentId);

                var result = await _mediator.Send(
                    new IdentifiedCommand<PromoteTreasurerCommand>(command, @event.Id));

                return result;
            }
        }
    }
}