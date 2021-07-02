using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Models;
using FundraiserManagement.Application.Members.Commands.DisenrollStudents;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using SharedKernel.Infrastructure.Abstractions.EventBus;
using SharedKernel.Infrastructure.Concretes.Models;
using static FundraiserManagement.Application.MediatorModule;

namespace FundraiserManagement.Application.IntegrationEvents.Incoming
{
    internal sealed class StudentsDisenrolledIntegrationEvent : IntegrationEvent
    {
        public IEnumerable<StudentDisenrollmentData> DisenrolledStudentsData { get; }

        public StudentsDisenrolledIntegrationEvent(IEnumerable<StudentDisenrollmentData> disenrolledStudentsData)
        {
            DisenrolledStudentsData = Guard.Against.NullOrEmpty(disenrolledStudentsData, nameof(disenrolledStudentsData));
        }
    }

    internal sealed class StudentsDisenrolledIntegrationEventHandler : IIntegrationEventHandler<StudentsDisenrolledIntegrationEvent>
    {

        private readonly ISender _mediator;
        private readonly ILogger<StudentsDisenrolledIntegrationEvent> _logger;

        public StudentsDisenrolledIntegrationEventHandler(
            ISender mediator,
            ILogger<StudentsDisenrolledIntegrationEvent> logger)
        {
            _mediator = Guard.Against.Null(mediator, nameof(mediator));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        public async Task<Result> Handle(StudentsDisenrolledIntegrationEvent @event)
        {
            var studentIds = @event.DisenrolledStudentsData.Where(d => d.IsActive)
                .Select(d => d.MemberId).ToList();

            if(!studentIds.Any())
                return Result.Success();

            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{AppName}"))
            {
                _logger.LogInformation(
                    "----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                    @event.Id, AppName, @event);

                var command = new DisenrollStudentsCommand(studentIds);

                var result = await _mediator.Send(
                    new IdentifiedCommand<DisenrollStudentsCommand>(command, @event.Id));

                return result;
            }
        }
    }
}