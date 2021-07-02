using System.Threading.Tasks;
using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Models;
using FundraiserManagement.Application.Members.Commands.DivestHeadmaster;
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
    internal sealed class HeadmasterDivestedIntegrationEvent : IntegrationEvent
    {
        public HeadmasterDivestedIntegrationEvent(MemberId headmasterId, bool isActive)
        {
            HeadmasterId = headmasterId;
            IsActive = isActive;
            RemovedRole = SchoolRole.Headmaster.ToString();
            AssignedRole = SchoolRole.Teacher.ToString();
        }

        public MemberId HeadmasterId { get; }
        public string RemovedRole { get; }
        public string AssignedRole { get; }
        public bool IsActive { get; }
    }

    internal sealed class HeadmasterDivestedIntegrationEventHandler : IIntegrationEventHandler<HeadmasterDivestedIntegrationEvent>
    {
        private readonly ISender _mediator;
        private readonly ILogger<HeadmasterDivestedIntegrationEventHandler> _logger;

        public HeadmasterDivestedIntegrationEventHandler(
            ISender mediator,
            ILogger<HeadmasterDivestedIntegrationEventHandler> logger)
        {
            _mediator = Guard.Against.Null(mediator, nameof(mediator));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        public async Task<Result> Handle(HeadmasterDivestedIntegrationEvent @event)
        {
            if (!@event.IsActive)
                return Result.Success();

            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{AppName}"))
            {
                _logger.LogInformation(
                    "----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                    @event.Id, AppName, @event);

                var command = new DivestHeadmasterCommand(@event.HeadmasterId);

                var result = await _mediator.Send(
                    new IdentifiedCommand<DivestHeadmasterCommand>(command, @event.Id));

                return result;
            }
        }
    }
}