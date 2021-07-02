using System.Threading.Tasks;
using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Models;
using FundraiserManagement.Application.Members.Commands.PromoteHeadmaster;
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
    internal sealed class HeadmasterPromotedIntegrationEvent : IntegrationEvent
    {
        public HeadmasterPromotedIntegrationEvent(MemberId teacherId, bool isActive)
        {
            TeacherId = teacherId;
            RemovedRole = SchoolRole.Teacher.ToString();
            AssignedRole = SchoolRole.Headmaster.ToString();
            IsActive = isActive;
        }

        public MemberId TeacherId { get; }
        public string RemovedRole { get; }
        public string AssignedRole { get; }
        public bool IsActive { get; }
    }

    internal sealed class HeadmasterPromotedIntegrationEventHandler : IIntegrationEventHandler<HeadmasterPromotedIntegrationEvent>
    {
        private readonly ISender _mediator;
        private readonly ILogger<HeadmasterPromotedIntegrationEventHandler> _logger;

        public HeadmasterPromotedIntegrationEventHandler(
            ISender mediator,
            ILogger<HeadmasterPromotedIntegrationEventHandler> logger)
        {
            _mediator = Guard.Against.Null(mediator, nameof(mediator));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        public async Task<Result> Handle(HeadmasterPromotedIntegrationEvent @event)
        {
            if (!@event.IsActive)
                return Result.Success();

            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{AppName}"))
            {
                _logger.LogInformation(
                    "----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                    @event.Id, AppName, @event);

                var command = new PromoteHeadmasterCommand(@event.TeacherId);

                var result = await _mediator.Send(
                    new IdentifiedCommand<PromoteHeadmasterCommand>(command, @event.Id));

                return result;
            }
        }
    }
}