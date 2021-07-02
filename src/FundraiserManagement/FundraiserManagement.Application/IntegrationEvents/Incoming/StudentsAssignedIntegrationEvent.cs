using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using FundraiserManagement.Application.Common.Models;
using FundraiserManagement.Application.Members.Commands.AssignStudents;
using FundraiserManagement.Domain.Common.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using SharedKernel.Infrastructure.Abstractions.EventBus;
using SharedKernel.Infrastructure.Concretes.Models;
using static FundraiserManagement.Application.MediatorModule;

namespace FundraiserManagement.Application.IntegrationEvents.Incoming
{
    internal sealed class StudentsAssignedIntegrationEvent : IntegrationEvent
    {
        public StudentsAssignedIntegrationEvent(GroupId groupId, IEnumerable<MemberIsActiveModel> membersData)
        {
            GroupId = groupId;
            MembersData = Guard.Against.NullOrEmpty(membersData, nameof(membersData)); ;
        }

        public GroupId GroupId { get; }
        public IEnumerable<MemberIsActiveModel> MembersData { get; }
    }

    internal sealed class StudentsAssignedIntegrationEventHandler : IIntegrationEventHandler<StudentsAssignedIntegrationEvent>
    {

        private readonly ISender _mediator;
        private readonly ILogger<StudentsAssignedIntegrationEvent> _logger;

        public StudentsAssignedIntegrationEventHandler(
            ISender mediator,
            ILogger<StudentsAssignedIntegrationEvent> logger)
        {
            _mediator = Guard.Against.Null(mediator, nameof(mediator));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        public async Task<Result> Handle(StudentsAssignedIntegrationEvent @event)
        {
            var studentIds = @event.MembersData.Where(d => d.IsActive)
                .Select(d => d.MemberId).ToList();
            if (!studentIds.Any())
                return Result.Success();

            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{AppName}"))
            {
                _logger.LogInformation(
                    "----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                    @event.Id, AppName, @event);

                var command = new AssignStudentsCommand(studentIds, @event.GroupId);

                var result = await _mediator.Send(
                    new IdentifiedCommand<AssignStudentsCommand>(command, @event.Id));

                return result;
            }
        }
    }
}