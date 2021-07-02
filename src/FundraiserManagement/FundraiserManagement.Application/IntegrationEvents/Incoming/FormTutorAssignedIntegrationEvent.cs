using System.Threading.Tasks;
using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Models;
using FundraiserManagement.Application.Members.Commands.PromoteFormTutor;
using FundraiserManagement.Domain.Common.Models;
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
    internal sealed class FormTutorAssignedIntegrationEvent : IntegrationEvent
    {
        public MemberId FormTutorId { get; }
        public GroupId GroupId { get; }
        public string AssignedRole { get; }
        public bool IsActive { get; }

        public FormTutorAssignedIntegrationEvent(MemberId formTutorId, GroupId groupId, bool isActive)
        {
            FormTutorId = formTutorId;
            GroupId = groupId;
            AssignedRole = GroupRoles.FormTutor;
            IsActive = isActive;
        }
    }

    internal sealed class FormTutorAssignedIntegrationEventHandler
        : IIntegrationEventHandler<FormTutorAssignedIntegrationEvent>
    {
        private readonly ISender _mediator;
        private readonly ILogger<FormTutorAssignedIntegrationEventHandler> _logger;

        public FormTutorAssignedIntegrationEventHandler(
            ISender mediator,
            ILogger<FormTutorAssignedIntegrationEventHandler> logger)
        {
            _mediator = Guard.Against.Null(mediator, nameof(mediator));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        public async Task<Result> Handle(FormTutorAssignedIntegrationEvent @event)
        {
            if (!@event.IsActive)
                return Result.Success();

            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{AppName}"))
            {
                _logger.LogInformation(
                    "----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                    @event.Id, AppName, @event);

                var command = new PromoteFormTutorCommand(@event.FormTutorId, @event.GroupId);

                var result = await _mediator.Send(new IdentifiedCommand<PromoteFormTutorCommand>(command, @event.Id));

                return result;
            }
        }
    }
}