using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Models;
using FundraiserManagement.Application.Members.Commands.DivestFormTutors;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using SharedKernel.Domain.Constants;
using SharedKernel.Infrastructure.Abstractions.EventBus;
using SharedKernel.Infrastructure.Concretes.Models;
using static FundraiserManagement.Application.MediatorModule;

namespace FundraiserManagement.Application.IntegrationEvents.Incoming
{
    internal sealed class FormTutorsDivestedIntegrationEvent : IntegrationEvent
    {
        public IEnumerable<MemberIsActiveModel> FormTutorsData { get; }
        public string RemovedRole { get; }

        public FormTutorsDivestedIntegrationEvent(IEnumerable<MemberIsActiveModel> formTutorsData)
        {
            FormTutorsData = Guard.Against.NullOrEmpty(formTutorsData, nameof(formTutorsData));
            RemovedRole = GroupRoles.FormTutor;
        }
    }

    internal sealed class FormTutorsDivestedIntegrationEventHandler
        : IIntegrationEventHandler<FormTutorsDivestedIntegrationEvent>
    {
        private readonly ISender _mediator;
        private readonly ILogger<FormTutorsDivestedIntegrationEventHandler> _logger;

        public FormTutorsDivestedIntegrationEventHandler(
            ISender mediator,
            ILogger<FormTutorsDivestedIntegrationEventHandler> logger)
        {
            _mediator = Guard.Against.Null(mediator, nameof(mediator));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        public async Task<Result> Handle(FormTutorsDivestedIntegrationEvent @event)
        {
            var memberIds = @event.FormTutorsData.Where(d => d.IsActive)
                .Select(id => id.MemberId).ToList();

            if (!memberIds.Any())
                return Result.Success();

            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{AppName}"))
            {
                _logger.LogInformation(
                    "----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                    @event.Id, AppName, @event);

                var command = new DivestFormTutorsCommand(memberIds);

                var result = await _mediator.Send(
                    new IdentifiedCommand<DivestFormTutorsCommand>(command, @event.Id));

                return result;
            }
        }
    }
}