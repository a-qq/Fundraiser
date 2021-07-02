using System.Threading.Tasks;
using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Models;
using FundraiserManagement.Application.Members.Commands.DivestFormTutor;
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
    internal sealed class FormTutorDivestedIntegrationEvent : IntegrationEvent
    {
        public FormTutorDivestedIntegrationEvent(MemberId formTutorId, bool isActive)
        {
            FormTutorId = formTutorId;
            IsActive = isActive;
            RemovedRole = GroupRoles.FormTutor;
        }

        public MemberId FormTutorId { get; }
        public string RemovedRole { get; }
        public bool IsActive { get; }
    }

    internal sealed class FormTutorDivestedIntegrationEventHandler
        : IIntegrationEventHandler<FormTutorDivestedIntegrationEvent>
    {
        private readonly ISender _mediator;
        private readonly ILogger<FormTutorDivestedIntegrationEventHandler> _logger;

        public FormTutorDivestedIntegrationEventHandler(
            ISender mediator,
            ILogger<FormTutorDivestedIntegrationEventHandler> logger)
        {
            _mediator = Guard.Against.Null(mediator, nameof(mediator));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        public async Task<Result> Handle(FormTutorDivestedIntegrationEvent @event)
        {
            if (!@event.IsActive)
                return Result.Success();

            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{AppName}"))
            {
                _logger.LogInformation(
                    "----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                    @event.Id, AppName, @event);

                var command = new DivestFormTutorCommand(@event.FormTutorId);

                var result = await _mediator.Send(new IdentifiedCommand<DivestFormTutorCommand>(command, @event.Id));

                return result;
            }
        }
    }
}