using System.Threading.Tasks;
using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Models;
using FundraiserManagement.Application.Members.Commands.AssignStudent;
using FundraiserManagement.Domain.Common.Models;
using FundraiserManagement.Domain.MemberAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using SharedKernel.Infrastructure.Abstractions.EventBus;
using SharedKernel.Infrastructure.Concretes.Models;
using static FundraiserManagement.Application.MediatorModule;

namespace FundraiserManagement.Application.IntegrationEvents.Incoming
{
    internal sealed class StudentAssignedIntegrationEvent : IntegrationEvent
    {
        public MemberId StudentId { get; }
        public GroupId GroupId { get; }
        public bool IsActive { get; }

        public StudentAssignedIntegrationEvent(MemberId studentId, GroupId groupId, bool isActive)
        {
            StudentId = studentId;
            GroupId = groupId;
            IsActive = isActive;
        }
    }

    internal sealed class StudentAssignedIntegrationEventHandler
        : IIntegrationEventHandler<StudentAssignedIntegrationEvent>
    {
        private readonly ISender _mediator;
        private readonly ILogger<StudentAssignedIntegrationEventHandler> _logger;

        public StudentAssignedIntegrationEventHandler(
            ISender mediator,
            ILogger<StudentAssignedIntegrationEventHandler> logger)
        {
            _mediator = Guard.Against.Null(mediator, nameof(mediator));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        public async Task<Result> Handle(StudentAssignedIntegrationEvent @event)
        {
            if (!@event.IsActive)
                return Result.Success();

            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{AppName}"))
            {
                _logger.LogInformation(
                    "----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                    @event.Id, AppName, @event);

                var command = new AssignStudentCommand(@event.StudentId, @event.GroupId);

                var result = await _mediator.Send(new IdentifiedCommand<AssignStudentCommand>(command, @event.Id));

                return result;
            }
        }
    }
}