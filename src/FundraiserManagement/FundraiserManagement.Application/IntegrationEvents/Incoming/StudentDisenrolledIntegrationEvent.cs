using System.Threading.Tasks;
using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Models;
using FundraiserManagement.Application.Members.Commands.DisenrollStudent;
using FundraiserManagement.Domain.MemberAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using SharedKernel.Infrastructure.Abstractions.EventBus;
using SharedKernel.Infrastructure.Concretes.Models;
using static FundraiserManagement.Application.MediatorModule;

namespace FundraiserManagement.Application.IntegrationEvents.Incoming
{
    internal sealed class StudentDisenrolledIntegrationEvent : IntegrationEvent
    {
        public MemberId StudentId { get; }
        public string RemovedRole { get; }
        public bool IsActive { get; }

        public StudentDisenrolledIntegrationEvent(MemberId studentId, string removedRole, bool isActive)
        {
            StudentId = studentId;
            RemovedRole = removedRole;
            IsActive = isActive;
        }
    }

    internal sealed class StudentDisenrolledIntegrationEventHandler : IIntegrationEventHandler<StudentDisenrolledIntegrationEvent>
    {

        private readonly ISender _mediator;
        private readonly ILogger<StudentDisenrolledIntegrationEvent> _logger;

        public StudentDisenrolledIntegrationEventHandler(
            ISender mediator,
            ILogger<StudentDisenrolledIntegrationEvent> logger)
        {
            _mediator = Guard.Against.Null(mediator, nameof(mediator));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        public async Task<Result> Handle(StudentDisenrolledIntegrationEvent @event)
        {
            if (!@event.IsActive)
                return Result.Success();

            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{AppName}"))
            {
                _logger.LogInformation(
                    "----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                    @event.Id, AppName, @event);

                var command = new DisenrollStudentCommand(@event.StudentId);

                var result = await _mediator.Send(
                    new IdentifiedCommand<DisenrollStudentCommand>(command, @event.Id));

                return result;
            }
        }
    }
}